using ECommerce.Api.Helper;
using ECommerce.Core.Entites.Basket;
using ECommerce.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace ECommerce.Api.Controllers
{
    [Route("api/payments")]
    [EnableRateLimiting("StrictPolicy")]
    [ApiController]
    [Authorize]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }


        [HttpPost]
        public async Task<ActionResult<CustomerBasket>> Create([FromQuery] string basketId, [FromQuery]int? deliveryMethodId) 
        {
            var basketResult = await _paymentService.CreateOrUpdatePaymentasync(basketId, deliveryMethodId);

            if (basketResult == null)
            {
                return BadRequest(new ResponseAPI<object>(400,
     "Failed to initialize payment for the basket. Note: Ensure the basket ID is correct and has not expired, and all product IDs is correct"));
            }

            return Ok(new ResponseAPI<CustomerBasket>(200, "Payment processing initiated successfully", basketResult));
        }
    }
}
