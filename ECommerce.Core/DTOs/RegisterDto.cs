using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.DTOs
{
    public record RegisterDto
    {
        [Required(ErrorMessage = "UserName is Required")]
        
        public string UserName { get; set; } = null!;

        [Required(ErrorMessage = "Email is Required")]
        [EmailAddress(ErrorMessage ="wrong format for email")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Password is Required")]
        public string Password { get; set; } = null!; 
        
    }
    public record AddUserDto : RegisterDto 
    { 
    public string? Role { get; set; }
    }
}
