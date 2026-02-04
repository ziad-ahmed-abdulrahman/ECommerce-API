using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.Entites.Basket
{
    public class CustomerBasket : BaseEntity<string>
    {
        public CustomerBasket()
        {
        }
        public CustomerBasket(string id)
        {
            this.Id = id; // Id is Key in redis
        }
        public string Id { get; set; }

        public string PaymentIntentId { get; set; }
        public string ClientSecret { get; set; }

        public List<BasketItem> BasketItems { get; set; } = new List<BasketItem>(); 
    }
}
