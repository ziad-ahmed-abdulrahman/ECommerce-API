using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.DTOs
{
    public class UpdateMeDto
    {
       
        public string? PhoneNumber { get; set; } 
        public string? FirstName { get; set; } 
        public string? LastName { get; set; }
        public string? City { get; set; }
        public string? CodeZip { get; set; } 
        public string? Street { get; set; } 
        public string? State { get; set; }
    }
    public class UpdateUserDto : UpdateMeDto
    {
        
        public bool? IsActive { get; set; }
    }
}
