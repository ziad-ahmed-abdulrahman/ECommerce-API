using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ECommerce.Core.Entites.Product
{
    public class Category : BaseEntity<int>
    {
        public String Name { get; set; } = null!; 

        public String Description { get; set; } = null!;

    }
}
