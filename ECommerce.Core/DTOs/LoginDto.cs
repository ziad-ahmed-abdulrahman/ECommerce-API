using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.DTOs
{

    public class LoginDto
    {
        [Required(ErrorMessage = "User Name is Required")]
       
        public string UserName { get; set; } = null!;

        [Required(ErrorMessage = "Password is Required")]
        [PasswordPropertyText]
        public string Password { get; set; } = null!;
    }

}
