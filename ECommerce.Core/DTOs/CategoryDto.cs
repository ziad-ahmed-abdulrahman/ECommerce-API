using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.DTOs
{
    public record CategoryDto  // for add / get
    {
        [Required]
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!; 
    }   
    public record UpdateCategoryDto  : CategoryDto// for update
    { 
        public int Id { get; set; }    
    }
   
}
