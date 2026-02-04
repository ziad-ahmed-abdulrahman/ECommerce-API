using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.DTOs
{
    public class VerifyCodeDto 
    {
        [DisplayName("Email")]
        [Required(ErrorMessage = "Email is Required")]
        [EmailAddress]
        public string? Email { get; set; } = null;

        [Required(ErrorMessage ="Code is Required")]
        public string? Code { get; set; }
        [PasswordPropertyText]
        [DisplayName("New Password")]
        public string? NewPassword { get; set; } = null;
        [DisplayName("Old Password")]
        public string? OldPassword { get; set; } = null;
    }
}
