using ECommerce.Core.Entites.Basket;
using ECommerce.Core.interfaces;

using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Repositories
{
    public class CustomerBasketRepositry : ICustomerBasketRepositry
    {
        private readonly IDatabase _database;

        public CustomerBasketRepositry(IConnectionMultiplexer redis)
        {
            _database = redis.GetDatabase();
        }

       

        public async Task<bool> DeleteBasketAsync(string id)
        {
            return await _database.KeyDeleteAsync(id);
        }

        public async Task<CustomerBasket> GetBasketAsync(string id)
        {
            
            var reaultBasket = await _database.StringGetAsync(id);
            if (!string.IsNullOrEmpty(reaultBasket)) 
            {
                return JsonSerializer.Deserialize<CustomerBasket>(reaultBasket!)!;
            }
            return null!; 

        }

        public async Task<CustomerBasket> AddOrUpdateBasketAsync(CustomerBasket customerBasket)
        {
            var basket = await _database.
                StringSetAsync(customerBasket.Id, JsonSerializer.Serialize(customerBasket),TimeSpan.FromDays(3));

            if (basket) 
                return await GetBasketAsync(customerBasket.Id);
          
            return null!; 
        }
    }
}
