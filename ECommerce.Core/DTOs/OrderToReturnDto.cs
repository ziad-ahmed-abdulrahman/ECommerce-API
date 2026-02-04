using ECommerce.Core.Entites.Order;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.DTOs
{
    public class OrderToReturnDto
    {
        public int Id {  get; set; }
        public string BuyerEmail { get; set; }
        [Required]
        public decimal SubTotal { get; set; }
        public decimal Total { get; set; }

        public DateTime OrderDate { get; set; } 

        public ShippingAddress ShippingAddress { get; set; }

        public string DeliveryMethod { get; set; }

        public IReadOnlyList<OrderItemDto> OrderItems { get; set; }

        public string PaymentStatus { get; set; }

    }

    public class OrderItemDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
