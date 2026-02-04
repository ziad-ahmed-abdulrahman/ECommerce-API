using ECommerce.Core.DTOs;
using ECommerce.Core.Entites.Order;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.Services
{
    public interface IOrderService
    {
        public Task<Order> CreateOrderAsync(OrderDto orderDto , string Email);
        public Task<IReadOnlyList<OrderToReturnDto>> GetAllOrdersForUserAsync(string Email);
        public Task<OrderToReturnDto> GeteOrderByIdAsync(int id , string Email);

        public Task<List<DeliveryMethod>> GetDeliveryMethodsAsync(); 
    }
}
