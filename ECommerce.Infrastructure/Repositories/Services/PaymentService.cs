using ECommerce.Core.Entites.Basket;
using ECommerce.Core.interfaces;
using ECommerce.Core.Services;
using ECommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Stripe;
using Stripe.V2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Repositories.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _dbContext;
        public PaymentService(IUnitOfWork unitOfWork, IConfiguration configuration, AppDbContext dbContext)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _dbContext = dbContext;
        }

        public async Task<CustomerBasket> CreateOrUpdatePaymentasync(string basketId, int? deliveryMethodId)
        {
            var basket = await _unitOfWork.CustomerBasketRepositry.GetBasketAsync(basketId);
            if (basket == null)
                return null!;
            StripeConfiguration.ApiKey = _configuration["StripeSetting:SecretKey"];
            decimal shippingPrice = 0m;
            if (deliveryMethodId.HasValue)
            {
                var deliveryMethod = await _dbContext.DeliveryMethods.AsNoTracking()
                    .FirstOrDefaultAsync(d => d.Id == deliveryMethodId);

                shippingPrice = deliveryMethod == null ? 0m : deliveryMethod.Price;
            }
            foreach (var item in basket.BasketItems)
            {
                var product = await _unitOfWork.ProductRepositry.GetByIdAsync(item.Id);

                if (product == null)
                {
                    return null!;
                }
                item.Price = product.NewPrice;

            }
            PaymentIntentService paymentIntentService = new PaymentIntentService();
            PaymentIntent _intent;

            if (string.IsNullOrEmpty(basket.PaymentIntentId))
            {
                var option = new PaymentIntentCreateOptions
                {
                    Amount = (long)basket.BasketItems.Sum(m => m.Quantity * (m.Price * 100)) + (long)(shippingPrice * 100),
                    Currency = "USD",
                    PaymentMethodTypes = new List<string> { "card" }
                };
                _intent = await paymentIntentService.CreateAsync(option);
                basket.PaymentIntentId = _intent.Id;
                basket.ClientSecret = _intent.ClientSecret;
            }
            else
            {
                var option = new PaymentIntentUpdateOptions
                {
                    Amount = (long)basket.BasketItems.Sum(m => m.Quantity * (m.Price * 100)) + (long)(shippingPrice * 100)
                };
                await paymentIntentService.UpdateAsync(basket.PaymentIntentId, option);
            }

            await _unitOfWork.CustomerBasketRepositry.AddOrUpdateBasketAsync(basket);
            return basket;
        }


    }
}
