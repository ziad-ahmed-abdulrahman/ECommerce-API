using AutoMapper;
using ECommerce.Core.DTOs;
using ECommerce.Core.Entites.Order;
using ECommerce.Core.interfaces;
using ECommerce.Core.Services;
using ECommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Repositories.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IPaymentService _paymentService;

        public OrderService(IUnitOfWork unitOfWork, AppDbContext dbContext, IMapper mapper, IPaymentService paymentService)
        {
            _unitOfWork = unitOfWork;
            _dbContext = dbContext;
            _mapper = mapper;
            _paymentService = paymentService;
        }


        public async Task<Order> CreateOrderAsync(OrderDto orderDto, string buyerEmail)
        {
            Core.Entites.Basket.CustomerBasket? basket = await _unitOfWork.CustomerBasketRepositry.GetBasketAsync(orderDto.BasketId);
            if (basket == null)
            {
                return null!;
            }
            List<OrderItem> orderItems = new List<OrderItem>();

            foreach (var item in basket.BasketItems)
            {
                var product = await _unitOfWork.ProductRepositry.GetByIdAsync(item.Id);

                if (product == null)
                    return null!;

                OrderItem orderItem = new OrderItem(item.Id
                    , product.Name
                    , item.Image
                    , item.Price
                    , item.Quantity);

                orderItems.Add(orderItem);
            }
            var deliveryMethod = await _dbContext.DeliveryMethods.FirstOrDefaultAsync(d => d.Id == orderDto.DeliveryMethodId);

            var subTotal = orderItems.Sum(d => d.Quantity * d.Price);

            var addrres = _mapper.Map<ShippingAddress>(orderDto.ShippingAddressDto);

            var ExistOrder = await _dbContext.Orders.Where(o => o.PaymentIntentId == basket.PaymentIntentId).FirstOrDefaultAsync();
            if (ExistOrder != null)
            {
                _dbContext.Orders.Remove(ExistOrder); await _dbContext.SaveChangesAsync();
                //await _dbContext.SaveChangesAsync();
                _paymentService.CreateOrUpdatePaymentasync(basket.PaymentIntentId, deliveryMethod.Id);
            }

            Order order = new(buyerEmail, subTotal, addrres, deliveryMethod, orderItems, basket.PaymentIntentId);
           

            await _dbContext.Orders.AddAsync(order);
            await _dbContext.SaveChangesAsync();

            await _unitOfWork.CustomerBasketRepositry.DeleteBasketAsync(orderDto.BasketId);

            return order;

        }

        public async Task<IReadOnlyList<OrderToReturnDto>> GetAllOrdersForUserAsync(string Email)
        {
            var orders = await _dbContext.Orders.Where(o => o.BuyerEmail == Email)
            .Include(inc => inc.OrderItems)
            .Include(inc => inc.DeliveryMethod)
            .ToListAsync();

            var ordersDto = _mapper.Map<IReadOnlyList<OrderToReturnDto>>(orders);
            return ordersDto;
        }

        public async Task<OrderToReturnDto> GeteOrderByIdAsync(int id, string Email)
        {
            var order = await _dbContext.Orders.Where(predicate: o => o.Id == id && o.BuyerEmail == Email)
                .Include(inc => inc.OrderItems)
                .Include(inc => inc.DeliveryMethod)
                .FirstOrDefaultAsync();

            var orderdto = _mapper.Map<OrderToReturnDto>(order);
            return orderdto;
        }

        public async Task<List<DeliveryMethod>> GetDeliveryMethodsAsync()
        {
            return await _dbContext.DeliveryMethods.AsNoTracking().ToListAsync();
        }


    }
}
