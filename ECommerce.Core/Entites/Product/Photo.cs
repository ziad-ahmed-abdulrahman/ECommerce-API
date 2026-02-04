using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ECommerce.Core.Entites.Product
{
   public class Photo : BaseEntity<int>
    {
        public string ImageName { get; set; } = null!;

        public int ProductId { get; set; }

    }
}
