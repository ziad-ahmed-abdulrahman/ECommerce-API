using AutoMapper;
using Azure;
using ECommerce.Api.Helper;
using ECommerce.Core.Account.Entites;
using ECommerce.Core.DTOs;
using ECommerce.Core.interfaces;
using ECommerce.Core.Services;
using ECommerce.Core.Sharing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Api.Controllers
{
    [Route("api/accounts")]
    [ApiController]
    public class AccountsController : BaseController
    {
        private readonly IEmailService _emailService;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signManager;
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _configure;
        public AccountsController(IUnitOfWork unitOfWork,
            IMapper mapper,
            IEmailService emailService,
            UserManager<AppUser> userManager
            , IWebHostEnvironment env,
            SignInManager<AppUser> signManager,
            IConfiguration configure) : base(unitOfWork, mapper)
        {
            _emailService = emailService;
            _userManager = userManager;
            _signManager = signManager;
            _env = env;
            _configure = configure;
        }

        #region Register EndPoint
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto userFromRequest)
        {
            
            if (userFromRequest == null)
                return BadRequest(new ResponseAPI<object>(400, "empty data"));

            AppUser? existingEmailUser = await _userManager.FindByEmailAsync(userFromRequest.Email);
            if (existingEmailUser != null)
                return BadRequest(new ResponseAPI<object>(400, "Email is already registered"));

            var existingUserName = await _userManager.FindByNameAsync(userFromRequest.UserName);
            if (existingUserName != null)
                return BadRequest(new ResponseAPI<object>(400, "Username is already taken"));

            AppUser user = new()
            {
                UserName = userFromRequest.UserName,
                Email = userFromRequest.Email,

            };

            IdentityResult CreateUserResult = await _userManager.CreateAsync(user, userFromRequest.Password);

            if (CreateUserResult.Succeeded)
            {

                var AddRoleResult = await _userManager.AddToRoleAsync(user, "User");

                if (AddRoleResult.Succeeded)
                {

                    var response = new ResponseAPI<object>(201, "Account created! You can activate it anytime. (No additional data to return)");

                    return CreatedAtAction(nameof(SendCode), response);
                }
                else
                {
                    return BadRequest(new ResponseAPI<IEnumerable<string>>(400, "Identity Errors Occurred", AddRoleResult.Errors.Select(e => e.Description)));
                }
            }
            else
            {
                return BadRequest(new ResponseAPI<IEnumerable<string>>(400, "User creation failed.", CreateUserResult.Errors.Select(e => e.Description)));
            }
        }

        #endregion
        //////////////////////////////
        //////////////////////////////
        //////////////////////////////
        #region Send Code EndPoint
        [HttpPost("send-code")]

        public async Task<IActionResult> SendCode(SendCodeDto sendCodeDto)
        {
            if (string.IsNullOrEmpty(sendCodeDto.Email))
                return BadRequest(new ResponseAPI<object>(400, "Email is required"));

            if (string.IsNullOrEmpty(sendCodeDto.Operation))
                return BadRequest(new ResponseAPI<object>(400, "Operation is required"));

            var user = await _userManager.FindByEmailAsync(sendCodeDto.Email);

            if (user == null)
                return BadRequest(new ResponseAPI<object>(400, "user not found"));


            string oneTimeCode = GenerateCode.GenerateActivationCode();
            double codeExpiryMinutes = Convert.ToDouble(_configure["OTPSetting:codeExpiryMinutes"]);
            user.OneTimeCode = oneTimeCode;
            user.OneTimeCodeExpiry = DateTime.UtcNow.AddMinutes(codeExpiryMinutes);


            string templatePath = default!;
            string body = String.Empty;
            string subject = String.Empty;
            string operation = sendCodeDto.Operation.ToLower();
            user.CodeOperation = operation;
            string message = String.Empty;

            switch (operation)
            {
                case "activate":
                    if (user.IsActive == true)
                        return BadRequest(new ResponseAPI<object>(400, "Your account is already active. You can log in now!"));
                    user.CodeOperation = "activate";
                    templatePath = Path.Combine(_env.ContentRootPath, "Templates", "ActivationEmail.html");
                    body = await System.IO.File.ReadAllTextAsync(templatePath);
                    body = body
                         .Replace("{{UserName}}", user.UserName)
                         .Replace("{{ActivationCode}}", oneTimeCode)
                         .Replace("{{ExpiryMinutes}}", codeExpiryMinutes.ToString());
                    subject = "Activate Your EComApp Account";
                    message = $"Account activation code sent." +
                              $" Valid for {codeExpiryMinutes} minutes.";

                    break;

                case "reset":
                    if (user.IsActive == false)
                    {
                        return BadRequest(new ResponseAPI<object>(400, "Your account is not active. You should activate it first!"));
                    }
                    user.CodeOperation = "reset";
                    templatePath = Path.Combine(_env.ContentRootPath, "Templates", "ResetPassword.html");
                    body = await System.IO.File.ReadAllTextAsync(templatePath);
                    body = body
                         .Replace("{{UserName}}", user.UserName)
                         .Replace("{{ResetCode}}", oneTimeCode)
                         .Replace("{{ExpiryMinutes}}", codeExpiryMinutes.ToString());
                    subject = "Reset Your EComApp Password";
                    message = $"Password reset code sent." +

                        $" Valid for {codeExpiryMinutes} minutes.";

                    break;

                case "changepassword":
                    if (user.IsActive == false)
                    {
                        return BadRequest(new ResponseAPI<object>(400, "Your account is not active. You should activate it first!"));
                    }
                    user.CodeOperation = "changepassword";
                    templatePath = Path.Combine(_env.ContentRootPath, "Templates", "ChangePassword.html");
                    body = await System.IO.File.ReadAllTextAsync(templatePath);
                    body = body
                          .Replace("{{UserName}}", user.UserName)
                          .Replace("{{ChangePasswordCode}}", oneTimeCode)
                          .Replace("{{ExpiryMinutes}}", codeExpiryMinutes.ToString());

                    subject = "Change Your EComApp Account Password";
                    message = $"Account Change Password code will sent." +
                              $" Valid for {codeExpiryMinutes} minutes.";

                    break;

                default:
                    return BadRequest(new ResponseAPI<object>(400, "invalid operation"));

            }
            await _userManager.UpdateAsync(user);
            await _emailService.SendEmailAsync(user.Email!, subject, body);

            return Ok(new ResponseAPI<object>(200, message));
        }
        #endregion
        //////////////////////////////
        //////////////////////////////
        //////////////////////////////
        #region Verify Code EndPoint
        [HttpPost("verify-code")]
        public async Task<IActionResult> VerifyCode(VerifyCodeDto verifyEmailDto)
        {
            if (string.IsNullOrEmpty(verifyEmailDto.Email))
                return BadRequest(new ResponseAPI<object>(400, "Email is required"));

            if (string.IsNullOrEmpty(verifyEmailDto.Code))
                return BadRequest(new ResponseAPI<object>(400, "Code is required"));

            AppUser? user = await _userManager.FindByEmailAsync(verifyEmailDto.Email);

            if (user == null)
                return BadRequest(new ResponseAPI<object>(400, "user not found"));

            if (user.OneTimeCodeExpiry < DateTime.UtcNow)
            {
                user.OneTimeCode = null;
                user.OneTimeCodeExpiry = null;
                await _userManager.UpdateAsync(user);
                return BadRequest(new ResponseAPI<object>(400, "code expired. Please request a new code."));
            }

            if (user.OneTimeCode != verifyEmailDto.Code)
                return BadRequest(new ResponseAPI<object>(400, "Invalid activation code."));



            if (string.IsNullOrEmpty(user.CodeOperation))
                return BadRequest(new ResponseAPI<object>(400, "Invalid Operation"));

            string operation = user.CodeOperation;

            string templatePath = default!;
            string body = String.Empty;
            string subject = String.Empty;
            string message = String.Empty;

            switch (operation)
            {
                case "activate":
                    if (user.IsActive == true)
                        return BadRequest(new ResponseAPI<object>(400, "Your account is already active. You can log in now!"));
                    user.IsActive = true;
                    user.OneTimeCodeExpiry = null;
                    user.OneTimeCode = null;

                    templatePath = Path.Combine(
                         _env.ContentRootPath,
                         "Templates",
                         "AccountActivated.html"
                    );

                    body = await System.IO.File.ReadAllTextAsync(templatePath);

                    body = body
                        .Replace("{{UserName}}", user.UserName)
                        .Replace("{{LoginUrl}}", nameof(Login));

                    subject = "Your EComApp Account Has Been Activated 🎉";
                    message = "Account activated successfully. The user can now log in.";

                    break;

                case "reset":
                    if (user.IsActive == false)
                    {
                        return BadRequest(new ResponseAPI<object>(400, "Your account is not active. You should activate it first!"));
                    }

                    if (string.IsNullOrWhiteSpace(verifyEmailDto.NewPassword))
                        return BadRequest(new ResponseAPI<object>(400, "New Password is required"));

                    user.OneTimeCodeExpiry = null;
                    user.OneTimeCode = null;

                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var resetResult = await _userManager.ResetPasswordAsync(user, token, verifyEmailDto.NewPassword);

                    if (!resetResult.Succeeded)
                    {
                        var errors = string.Join(", ", resetResult.Errors.Select(e => e.Description));
                        return BadRequest(new ResponseAPI<object>(400, $"Password reset failed: {errors}"));
                    }


                    templatePath = Path.Combine(_env.ContentRootPath, "Templates", "PasswordReseted.html");
                    body = await System.IO.File.ReadAllTextAsync(templatePath);
                    body = body
                         .Replace("{{UserName}}", user.UserName)
                         .Replace("{{LoginUrl}}", nameof(Login));

                    subject = "Your EComApp Password Has Been Reset 🔐";
                    message = "Password reset successfully. The user can now log in.";

                    break;

                case "changepassword":
                    if (user.IsActive == false)
                    {
                        return BadRequest(new ResponseAPI<object>(400, "Your account is not active. You should activate it first!"));
                    }

                    if (string.IsNullOrWhiteSpace(verifyEmailDto.NewPassword))
                        return BadRequest(new ResponseAPI<object>(400, "New Password is required"));

                    if (string.IsNullOrWhiteSpace(verifyEmailDto.OldPassword))
                        return BadRequest(new ResponseAPI<object>(400, "Old Password is required"));

                    user.OneTimeCodeExpiry = null;
                    user.OneTimeCode = null;

                    var changeResult = await _userManager.ChangePasswordAsync(user, verifyEmailDto.OldPassword, verifyEmailDto.NewPassword);

                    if (!changeResult.Succeeded)
                    {
                        var errors = string.Join(", ", changeResult.Errors.Select(e => e.Description));
                        return BadRequest(new ResponseAPI<object>(400, $"Password reset failed: {errors}"));
                    }

                    templatePath = Path.Combine(_env.ContentRootPath, "Templates", "PasswordChanged.html");
                    body = await System.IO.File.ReadAllTextAsync(templatePath);
                    body = body
                         .Replace("{{UserName}}", user.UserName);

                    subject = "Your EComApp Password Has Been Changed 🔐";
                    message = "Password changed successfully.";

                    break;


                default:
                    return BadRequest(new ResponseAPI<object>(400, "invalid operation."));
            }

            await _userManager.UpdateAsync(user);
            await _emailService.SendEmailAsync(user.Email!, subject, body);

            return Ok(new ResponseAPI<object>(200, message));
        }

        #endregion
        //////////////////////////////
        //////////////////////////////
        //////////////////////////////
        #region Login
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto userFromRequest)
        {
            if (string.IsNullOrWhiteSpace(userFromRequest.UserName))
                return BadRequest(new ResponseAPI<object>(400, "User Name is Required"));

            AppUser? user = await _userManager.FindByNameAsync(userFromRequest.UserName);

            if (user == null)
                return BadRequest(new ResponseAPI<object>(400, "UserName or Password is incorrect"));

            bool passwordValid = await _userManager.CheckPasswordAsync(user, userFromRequest.Password);

            if (user.IsActive == false)
                return BadRequest(new ResponseAPI<object>(400, "Please activate account first"));

            if (!passwordValid)
                return BadRequest(new ResponseAPI<object>(400, "UserName or Password is incorrect"));


            // add claims 
            List<Claim> userClaims = new List<Claim>();

            userClaims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
            userClaims.Add(new Claim(ClaimTypes.Email, user.Email!));
            userClaims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));

            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var roleName in userRoles)
            {
                userClaims.Add(new Claim(ClaimTypes.Role, roleName));
            }

            //add credentials
            SymmetricSecurityKey symmKey = new(key: Encoding.UTF8.GetBytes(_configure["JWT:SecritKey"]!));
            SigningCredentials signCred = new(symmKey, SecurityAlgorithms.HmacSha256);

            var tokenExpiration = Convert.ToDouble(_configure["TokenSetting:ExpirationInHours"]);
            //design token 
            JwtSecurityToken token = new JwtSecurityToken(
                   audience: _configure["JWT:AudienceIP"],
                   issuer: _configure["JWT:IssuerIP"],
                   expires: DateTime.Now.AddHours(value: tokenExpiration),
                   claims: userClaims,
                   signingCredentials: signCred
                   );


            //generate token response 
           
            return Ok(new ResponseAPI<object>(200, $"Login successful, {tokenExpiration} hour token", new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = DateTime.Now.AddHours(tokenExpiration)
            }));

        }



        #endregion
        //////////////////////////////
        //////////////////////////////
        //////////////////////////////
        #region Send Mail 
        [Authorize(Roles = "Admin")]
        [HttpPost("send-mail")]
        public async Task<IActionResult> Sendemail([FromForm] SendMailDto sendMmailDto)
        {
            if (string.IsNullOrWhiteSpace(sendMmailDto.To))
                return BadRequest(new ResponseAPI<object>(400, "Recipient email is required"));

            if (string.IsNullOrWhiteSpace(sendMmailDto.Body))
                return BadRequest(new ResponseAPI<object>(400, "Body is required"));

            if (string.IsNullOrWhiteSpace(sendMmailDto.Subject))
                return BadRequest(new ResponseAPI<object>(400, "Subject is required"));

            await _emailService.SendEmailAsync(sendMmailDto.To, sendMmailDto.Subject, sendMmailDto.Body, sendMmailDto.attachments);

            return Ok(new ResponseAPI<object>(200, "Email sent successfully"));
        }


        #endregion
        //////////////////////////////
        //////////////////////////////
        //////////////////////////////
        #region Get Me
        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetMe()
        {

            var claim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            if (claim == null)
                return StatusCode(401, new ResponseAPI<object>(401, "Your session has expired. Please log in again."));

            AppUser? user = await _userManager.Users
             .Include(u => u.Address)
             .FirstOrDefaultAsync(u => u.Email == claim);

            if (user.IsActive == false)
            {
                return BadRequest(new ResponseAPI<object>(400, "Your account is not active. You should activate it first!"));
            }

            if (user.Address == null)
                user.Address = new Address();


            return Ok(new ResponseAPI<object>(200, "User data retrieved successfully", new
            {
                Email = user?.Email,
                UserName = user?.UserName,
                Id = user?.Id,
                PhoneNumber = user?.PhoneNumber,
                Address = new
                {
                    FirstName = user?.Address?.FirstName,
                    LastName = user?.Address?.LastName,
                    City = user?.Address?.City,
                    CodeZip = user?.Address?.CodeZip,
                    Street = user?.Address?.Street,
                    State = user?.Address?.State,
                }
            }));

        }
        #endregion
        //////////////////////////////
        //////////////////////////////
        //////////////////////////////
        #region Update Me
        [Authorize]
        [HttpPatch]

        public async Task<IActionResult> UpdateMe(UpdateMeDto updateMeDto)
        {
            if (updateMeDto == null)
                return BadRequest(new ResponseAPI<object>(400, "Empty data: please provide the fields to be updated"));

            var claim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            if (claim == null)
                return StatusCode(401, new ResponseAPI<object>(401, "Your session has expired. Please log in again."));

            AppUser? user = await _userManager.Users
                .Include(u => u.Address)
                .FirstOrDefaultAsync(u => u.Email == claim);

            if (user?.IsActive == false)
            {
                return BadRequest(new ResponseAPI<object>(400, "Your account is not active. You should activate it first!"));
            }

            if (user.Address == null)
                user.Address = new Address();
            user.Address.FirstName = updateMeDto.FirstName ?? user.Address.FirstName;
            user.Address.LastName = updateMeDto.LastName ?? user.Address.LastName;
            user.Address.City = updateMeDto.City ?? user.Address.City;
            user.Address.CodeZip = updateMeDto.CodeZip ?? user.Address.CodeZip;
            user.Address.Street = updateMeDto.Street ?? user.Address.Street;
            user.Address.State = updateMeDto.State ?? user.Address.State;
            user.PhoneNumber = updateMeDto.PhoneNumber ?? user.PhoneNumber;

            await _userManager.UpdateAsync(user);
            return Ok(new ResponseAPI<object>(200, "Updated Successfully"));

        }

        #endregion
        //////////////////////////////
        //////////////////////////////
        //////////////////////////////
        #region Delete Me
        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> DeleteMe()
        {
            var claim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            if (claim == null)
                return StatusCode(401, new ResponseAPI<object>(401, "Your session has expired. Please log in again."));

            var user = await _userManager.FindByEmailAsync(claim);
            if (user == null || !user.IsActive)
            {
                return BadRequest(new ResponseAPI<object>(400, "This account is not activated."));
            }

            user.IsActive = false;
            await _userManager.UpdateAsync(user);

            return Ok(new ResponseAPI<object>(200, "Deleted successfully"));


        }
        #endregion
        //////////////////////////////
        //////////////////////////////
        //////////////////////////////


        #region Admin Panel
        #region  Add User
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddUser(AddUserDto userFromRequest)
        {
            if (userFromRequest == null)
                return BadRequest(new ResponseAPI<object>(400, "Empty data: please provide user details"));

            AppUser? existingEmailUser = await _userManager.FindByEmailAsync(userFromRequest.Email);
            if (existingEmailUser != null)
                return BadRequest(new ResponseAPI<object>(400, "Email is already registered"));

            var existingUserName = await _userManager.FindByNameAsync(userFromRequest.UserName);
            if (existingUserName != null)
                return BadRequest(new ResponseAPI<object>(400, "Username is already taken"));

            if (userFromRequest.Role != "User" && userFromRequest.Role != "Admin")
                return BadRequest(new ResponseAPI<object>(400, "Please choose a correct Role"));

            AppUser user = new()
            {
                UserName = userFromRequest.UserName,
                Email = userFromRequest.Email,
                IsActive = true
            };

            IdentityResult CreateUserResult = await _userManager.CreateAsync(user, userFromRequest.Password);

            if (CreateUserResult.Succeeded)
            {
                var AddRoleResult = await _userManager.AddToRoleAsync(user, userFromRequest.Role);

                if (AddRoleResult.Succeeded)
                {
                    return Ok(new ResponseAPI<object>(200, "Account created and activated successfully!"));
                }

                // Using string.Join to keep the message property a clean string
                var roleErrors = string.Join(", ", AddRoleResult.Errors.Select(e => e.Description));
                return BadRequest(new ResponseAPI<object>(400, roleErrors));
            }

            var userErrors = string.Join(", ", CreateUserResult.Errors.Select(e => e.Description));
            return BadRequest(error: new ResponseAPI<object>(400, userErrors));
        }
        #endregion
        //////////////////////////////
        //////////////////////////////
        //////////////////////////////
        #region Get User
        [Authorize(Roles ="Admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest(new ResponseAPI<object>(400, "Invalid ID"));

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound(new ResponseAPI<object>(404, "user not found"));
            var address = await _unitOfWork.AddressRepositry.GetByUserIdAsync(user.Id);
            user.Address = address!;

            return Ok(new ResponseAPI<object>(200, "User data retrieved successfully", new
            {
                Email = user?.Email,
                UserName = user?.UserName,
                Id = user?.Id,
                IsActive = user?.IsActive,
                PhoneNumber = user?.PhoneNumber,
                Address = new
                {
                    FirstName = user?.Address?.FirstName,
                    LastName = user?.Address?.LastName,
                    City = user?.Address?.City,
                    CodeZip = user?.Address?.CodeZip,
                    Street = user?.Address?.Street,
                    State = user?.Address?.State,
                }
            }));

        }

        #endregion
        //////////////////////////////
        //////////////////////////////
        //////////////////////////////
        #region Get all Users (support sort, search, pagination)
        [HttpGet]
        [Authorize(Roles = "Admin")] // Only admin can access the users
        public async Task<IActionResult> GetAll([FromQuery] GetUsersParams getUsersParams)
        {
            // Base query on the Users table
            var query = _userManager.Users
                        .AsNoTracking()
                        .AsQueryable();

            // Filter by search keyword
            if (!string.IsNullOrEmpty(getUsersParams.Search))
            {
                var searchWords = getUsersParams.Search.Split(' ');
                query = query.Where(u => searchWords.All(word =>
                    u.UserName.ToLower().Contains(word.ToLower()) ||
                    u.Email.ToLower().Contains(word.ToLower())
                ));
            }

            // Sorting
            if (!string.IsNullOrEmpty(getUsersParams.Sort))
            {
                query = getUsersParams.Sort switch
                {
                    "NameAsc" => query.OrderBy(u => u.UserName),
                    "NameDesc" => query.OrderByDescending(u => u.UserName),
                    "EmailAsc" => query.OrderBy(u => u.Email),
                    "EmailDesc" => query.OrderByDescending(u => u.Email),
                    _ => query.OrderBy(u => u.UserName) // Default sort
                };
            }
            else
            {
                query = query.OrderBy(u => u.UserName); // Default sort
            }

            // Total count before pagination
            var totalCount = await query.CountAsync();

            var totalPages = (int)Math.Ceiling((double)totalCount / getUsersParams.PageSize);

            // Pagination
            var users = await query
                .Skip((getUsersParams.PageNumber - 1) * getUsersParams.PageSize)
                .Take(getUsersParams.PageSize)
                .ToListAsync();

            // Map users to DTO (or anonymous object)
            var usersDto = users.Select(u => new
            {
                u.Id,
                u.UserName,
                u.Email,
                u.IsActive
            }).ToList();

            // Return the result
            return Ok(new ResponseAPI<object>(200,
     totalCount == 0
    ? "Welcome! Our user community is just starting, no users found yet."
    : (getUsersParams.PageNumber > totalPages
        ? $"You've reached the end of the list. We only have {totalPages} pages of users."
        : "Users retrieved successfully!"),
            new
            {
                TotalCount = totalCount,
                PageSize = getUsersParams.PageSize,
                TotalPages = totalPages,
                PageNumber = getUsersParams.PageNumber,
                Data = usersDto
            }));
        }
        //////////////////////////////
        //////////////////////////////
        //////////////////////////////
        #endregion
        //////////////////////////////
        //////////////////////////////
        //////////////////////////////
        #region Update User
        [Authorize(Roles = "Admin")]
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateUser([FromRoute] string id, UpdateUserDto updateUserDto)
        {
            if (updateUserDto == null)
                return BadRequest(new ResponseAPI<object>(400, "empty data"));

            if (id == null)
                return BadRequest(new ResponseAPI<object>(400, "Invalid Id"));

            AppUser? user = await _userManager.Users
              .Include(u => u.Address)
              .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                return NotFound(new ResponseAPI<object>(404, "User not found"));

            if (user.Address == null)
                user.Address = new Address();

            user.Address.FirstName = updateUserDto.FirstName ?? user.Address.FirstName;
            user.Address.LastName = updateUserDto.LastName ?? user.Address.LastName;
            user.Address.City = updateUserDto.City ?? user.Address.City;
            user.Address.CodeZip = updateUserDto.CodeZip ?? user.Address.CodeZip;
            user.Address.Street = updateUserDto.Street ?? user.Address.Street;
            user.Address.State = updateUserDto.State ?? user.Address.State;
            user.PhoneNumber = updateUserDto.PhoneNumber ?? user.PhoneNumber;
            user.IsActive = updateUserDto.IsActive ?? user.IsActive;


            await _userManager.UpdateAsync(user);

            return Ok(new ResponseAPI<object>(200, "updated successfully"));

        }

        #endregion
        //////////////////////////////
        //////////////////////////////
        //////////////////////////////
        #region Delete User
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> HardDeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound(new ResponseAPI<object>(404, "User not found"));

            // hard delete

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return BadRequest(new ResponseAPI<object>(400, errors));
            }

            return Ok(new ResponseAPI<object>(200, "Deleted successfully"));
        }

        #endregion
        //////////////////////////////
        //////////////////////////////
        //////////////////////////////

        #endregion

    }
}
