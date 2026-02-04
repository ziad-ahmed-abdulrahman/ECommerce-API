using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.Entites.Product
{
    public class Product : BaseEntity<int>
    {
        public string Name { get; set; } = null!; 

        public string Description { get; set; } = null!;

        public decimal NewPrice { get; set; }
        public decimal? OldPrice { get; set; }

        public int CategoryId { get; set; }

        [ForeignKey(nameof(CategoryId))]    
        public virtual Category Category { get; set; } = null!; 

        public virtual List<Photo> Photos { get; set; } = new List<Photo>(); 
        

    }
}
