using ECommerce.Api.Helper;
using ECommerce.Api.Mapping;
using ECommerce.Api.Middlewares;
using ECommerce.Core.Account.Entites;
using ECommerce.Core.Entites;
using ECommerce.Core.Services;
using ECommerce.Core.Sharing;
using ECommerce.Infrastructure;
using ECommerce.Infrastructure.Data;
using ECommerce.Infrastructure.Repositories.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using System.Threading.RateLimiting; 
using StackExchange.Redis;
using System.Text;
namespace ECommerce.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            // ============================================================
            // Rate Limiting Configuration with Unified Response
            // Configures API throttling and ensures a consistent 429 error format.
            // ============================================================
            builder.Services.AddRateLimiter(options =>
            {
               
                options.AddPolicy("StrictPolicy", httpContext =>
                {
                    var remoteIp = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

                    return RateLimitPartition.GetFixedWindowLimiter(remoteIp, _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 5,            
                        Window = TimeSpan.FromMinutes(1), 
                        QueueLimit = 0
                    });
                });

               
                options.AddPolicy("GeneralPolicy", partitioner: httpContext =>
                {
                    var remoteIp = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

                    return RateLimitPartition.GetFixedWindowLimiter(remoteIp, _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 15,           
                        Window = TimeSpan.FromSeconds(10), 
                        QueueLimit = 0
                    });
                });

                options.OnRejected = async (context, token) =>
                {
                    context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    context.HttpContext.Response.ContentType = "application/json";
                    var response = new ResponseAPI<object>(429, "Too many requests. Please slow down.");
                    await context.HttpContext.Response.WriteAsJsonAsync(response, token);
                };
            });
            // 1. API Controllers & Swagger Documentation Services
            // Registers controllers and configures Swagger for API testing.
            // ============================================================
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // ============================================================
            // 2. File System & Image Management Services
            // Handles physical file storage access and image processing logic.
            // ============================================================
            var rootPath = builder.Environment.WebRootPath ?? Path.Combine(builder.Environment.ContentRootPath, "wwwroot");

            if (!Directory.Exists(rootPath))
            {
                Directory.CreateDirectory(rootPath);
            }

            builder.Services.AddSingleton<IFileProvider>(new PhysicalFileProvider(rootPath));

            builder.Services.AddScoped<IImageManagementService, ImageManagementService>();

            // ============================================================
            // 3. Business Logic Infrastructure Services
            // Registers core application services for Emails, Orders, and Payments.
            // ============================================================
            builder.Services.AddTransient<IEmailService, EmailService>();
            builder.Services.AddTransient<IOrderService, OrderService>();
            builder.Services.AddTransient<IPaymentService, PaymentService>();

            // ============================================================
            // 4. Redis Cache & Distributed Memory Infrastructure
            // Establishes a singleton connection to Redis for high-speed caching.
            // ============================================================
            var redisConnString = builder.Configuration["ConnectionStrings:redis"];
            builder.Services.AddSingleton<IConnectionMultiplexer>(i =>
            {
                return ConnectionMultiplexer.Connect(configuration: redisConnString);
            });

            // ============================================================
            // 5. Configuration Settings & Options Pattern
            // Maps email settings from appsettings.json to the EmailSettings class.
            // ============================================================
            builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

            // ============================================================
            // 6. Identity Membership & Security Configuration
            // Sets up User/Role management, password rules, and DB persistence.
            // ============================================================
            builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.SignIn.RequireConfirmedEmail = false;
                options.Password.RequiredLength = 8;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();


            // ============================================================
            // 7. Authentication & JWT Bearer Configuration
            // Sets up JWT as the default scheme and defines token validation rules.
            // ============================================================
            #region Authentication & JWT
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; // check by token 
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; //unauthorize
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new()
                {
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration["JWT:IssuerIP"],
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["JWT:AudienceIP"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:SecritKey"]!))
                };
            });
            #endregion

            // ============================================================
            // 8. Cross-Origin Resource Sharing (CORS) Policy
            // Configures permissions for external domains to access the API.
            // ============================================================
            #region CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin", policy =>
                {
                    if (builder.Environment.IsDevelopment())
                    {

                        policy.AllowAnyOrigin()
                              .AllowAnyHeader()
                              .AllowAnyMethod();
                    }
                    else
                    {

                        var origin1 = builder.Configuration["Cors:FirstOrgin"] ?? "https://waiting-for-origin.com";
                        var origin2 = builder.Configuration["Cors:SecondOrgin"] ?? "https://waiting-for-origin.com";

                        policy.WithOrigins(origin1, origin2)
                              .AllowAnyHeader()
                              .AllowAnyMethod()
                              .AllowCredentials();
                    }
                });
            });
            #endregion

            // ============================================================
            // 9. Custom Infrastructure & Extension Methods
            // Injects additional project-specific services from external layers.
            // ============================================================
            builder.Services.InfrastructureConfiguration(builder.Configuration);

            // ============================================================
            // 10. AutoMapper Object Mapping Configuration
            // Scans the assembly for mapping profiles to simplify DTO conversions.
            // ============================================================
            builder.Services.AddAutoMapper(cfg => { }, typeof(Program).Assembly);


            // ============================================================
            // 11. Custom API Behavior & Model Validation Handling
            // Overrides the default 400 Bad Request response for validation errors.
            // ============================================================
            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    // Extracts error messages from the ModelState for all failed validation fields
                    var errors = context.ModelState
                        .Where(x => x.Value?.Errors.Count > 0)
                        .Select(x => new { Field = x.Key, Message = x.Value.Errors.First().ErrorMessage })
                        .ToList();

                    // Constructs a unified response object to keep consistency across the API
                    var result = new
                    {
                        IsSuccess = false,
                        StatusCode = 400,
                        Message = "Validation failed",
                        Errors = errors.Select(e => e.Message)
                    };

                    return new BadRequestObjectResult(result);
                };
            });

            // ============================================================
            // 12. Controller Services Registration
            // Enables the MVC controller architecture for handling HTTP requests.
            // ============================================================
            builder.Services.AddControllers();



            // ============================================================
            // 1. Build the Web Application Host
            // Finalizes service registrations and prepares the app to handle requests.
            // ============================================================
            var app = builder.Build();

            // ============================================================
            // 2. Global Exception Handling Middleware
            // The first line of defense that catches any errors and returns a unified JSON response.
            // ============================================================
            app.UseMiddleware<ExceptionMiddleware>();

            // ============================================================
            // 3. Rate Limiting Middleware
            // Throttles requests early in the pipeline to protect server resources.
            // ============================================================
            app.UseRateLimiter();

            // ============================================================
            // 3. Static Files Serving
            // Enables the API to serve physical assets like images and CSS from wwwroot.
            // ============================================================
            app.UseStaticFiles();

            // ============================================================
            // 4. API Documentation (Development Mode Only)
            // Generates the Swagger UI for testing endpoints during the coding phase.
            // ============================================================
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // ============================================================
            // 5. HTTPS Redirection
            // Automatically forces all incoming HTTP requests to use the secure HTTPS protocol.
            // ============================================================
            app.UseHttpsRedirection();




            app.UseCors("AllowSpecificOrigin");
            // ============================================================
            // 6. Authorization Middleware
            // Validates user permissions and ensures only authorized requests reach the controllers.
            // ============================================================
            app.UseAuthentication();
            app.UseAuthorization();

            // ============================================================
            // 7. Endpoint Mapping & Routing
            // Matches the incoming URL to the correct Controller and Action to execute logic.
            // ============================================================
            app.MapControllers();

            // ============================================================
            // 8. Application Execution
            // Starts the server and begins listening for incoming network traffic.
            // ============================================================
            app.Run();
        }
    }
}
