using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.DTOs
{
    public class SendCodeDto
    {
        [Required(ErrorMessage = "Email is required")]
        [MinLength(1)]
        [EmailAddress]
        public string? Email { get; set; }
        [Required(ErrorMessage ="operation is required")]
        [MinLength(1)]
        public string? Operation { get; set; }
    }
}
