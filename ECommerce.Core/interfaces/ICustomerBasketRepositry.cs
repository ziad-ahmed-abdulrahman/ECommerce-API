using ECommerce.Core.Entites.Basket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.interfaces
{
    public interface ICustomerBasketRepositry
    {
        public Task<CustomerBasket> GetBasketAsync(string id);
        public Task<CustomerBasket> AddOrUpdateBasketAsync(CustomerBasket customerBasket);
        public Task<bool> DeleteBasketAsync(string id);
    }
}
