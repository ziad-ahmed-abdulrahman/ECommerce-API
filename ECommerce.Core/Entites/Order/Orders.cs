using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.Entites.Order
{
    public  class Order : BaseEntity<int>
    {
        public Order()
        {
        }

        public Order(string buyerEmail, decimal subTotal, ShippingAddress shippingAddress, DeliveryMethod deliveryMethod, IReadOnlyList<OrderItem> orderItems, string paymentIntentId)
        {
            BuyerEmail = buyerEmail;
            SubTotal = subTotal;
            ShippingAddress = shippingAddress;
            DeliveryMethod = deliveryMethod;
            OrderItems = orderItems;
            PaymentIntentId = paymentIntentId;
        }
        public string PaymentIntentId { get; set; }

        [Required]
        [EmailAddress]  
        public string BuyerEmail {  get; set; }
        [Required]
        public decimal SubTotal { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;

        public ShippingAddress ShippingAddress { get; set; }

        public DeliveryMethod DeliveryMethod { get; set; }

        public IReadOnlyList<OrderItem> OrderItems { get; set; }

        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;

        
        public decimal GetTotal() 
        { 
            return SubTotal + DeliveryMethod.Price;
        }
    }
}
