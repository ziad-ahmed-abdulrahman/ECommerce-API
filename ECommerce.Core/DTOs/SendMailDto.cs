using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.DTOs
{
    public class SendMailDto
    {
        [Required]
        [EmailAddress(ErrorMessage ="wrong email format")]
        public string To { get; set; }
        [Required]

        public string Subject { get; set; }
        [Required]
         public string Body { get; set; }

        public IList<IFormFile>? attachments {  get; set; }

        

    }
}
