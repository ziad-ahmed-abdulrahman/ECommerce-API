using ECommerce.Core.Entites.Order;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ECommerce.Core.DTOs
{
    public record OrderDto
    {
        public int DeliveryMethodId { get; set; }
        [MaxLength(50)]
        public string BasketId { get; set; }

        [ JsonPropertyName("ShippingAddress")]
        public ShippingAddressDto ShippingAddressDto { get; set; }
    }

   
    public record ShippingAddressDto
    {
        [MaxLength(50)]
        public string FirstName { get; set; } = null!;
        [MaxLength(50)]

        public string LastName { get; set; } = null!;
        [MaxLength(50)]

        public string City { get; set; } = null!;
        [MaxLength(50)]

        public string CodeZip { get; set; } = null!;
        [MaxLength(50)]

        public string Street { get; set; } = null!;
        [MaxLength(50)]

        public string State { get; set; } = null!;
    }
}
