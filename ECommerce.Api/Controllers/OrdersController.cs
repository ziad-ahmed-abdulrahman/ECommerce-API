using ECommerce.Api.Helper;
using ECommerce.Core.DTOs;
using ECommerce.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe.Climate;
using System.Security.Claims;

namespace ECommerce.Api.Controllers
{
    [Route("api/orders")]
    [ApiController]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        [HttpPost]
        public async Task<IActionResult> CreateOrder(OrderDto orderDto) 
        {
            var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            var order = await _orderService.CreateOrderAsync(orderDto, email!);

            if (order == null)
            {
                return BadRequest(new ResponseAPI<object>(400, "Order creation failed. Please ensure the basket is valid and all products are available."));
            }
            return Ok(new ResponseAPI<ECommerce.Core.Entites.Order.Order?>(200, "Order created successfully", order));
        }

        [HttpGet]
        public async Task<IActionResult> GetOrdersForUser() 
        {
            var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            IReadOnlyList<OrderToReturnDto>? ordersDto = await _orderService.GetAllOrdersForUserAsync(email!);

            if (ordersDto == null)
            {
                return BadRequest(new ResponseAPI<object>(400, "An error occurred while retrieving orders."));
            }

            if (ordersDto.Count == 0)
            {
                return Ok(new ResponseAPI<object>(200, "You don't have any orders yet.", new
                {
                    Count = 0,
                    Orders = ordersDto
                }));
            }

            return Ok(new ResponseAPI<object>(200, "Orders retrieved successfully", new
            {
                Count = ordersDto.Count,
                Orders = ordersDto
            }));


        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrdersForUser(int id) 
        {
            var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            OrderToReturnDto? orderDto = await _orderService.GeteOrderByIdAsync(id, email!);

            if (orderDto == null)
            {
                return NotFound(new ResponseAPI<object>(404, "Order not found."));
            }

            return Ok(new ResponseAPI<OrderToReturnDto>(200, "Order retrieved successfully", orderDto));


        }
        [HttpGet("delivery-methods")]
        public async Task<IActionResult> GetDeliveryMethods() 
        {
            var deliveryMethods = await _orderService.GetDeliveryMethodsAsync();

            if (deliveryMethods == null || deliveryMethods.Count == 0)
            {
                return Ok(new ResponseAPI<object>(200, "No delivery methods found.", new
                {
                    Count = 0,
                    DeliveryMethods = new List<object>()
                }));
            }

            return Ok(new ResponseAPI<object>(200, "Delivery methods retrieved successfully", new
            {
                Count = deliveryMethods.Count,
                DeliveryMethods = deliveryMethods
            }));


        }
    }
}
