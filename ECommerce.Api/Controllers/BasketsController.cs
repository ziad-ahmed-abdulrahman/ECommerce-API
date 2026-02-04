using AutoMapper;
using ECommerce.Api.Helper;
using ECommerce.Core.Entites.Basket;
using ECommerce.Core.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace ECommerce.Api.Controllers
{
    [Route("api/baskets")]
    [ApiController]
    public class BasketsController : BaseController
    {
        public BasketsController(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id) 
        {
            var resultBasket = await _unitOfWork.CustomerBasketRepositry.GetBasketAsync(id);

            if (resultBasket == null)
            {
                
                return Ok(new ResponseAPI<object>(
                    statuscode: 400,
                    message: "Basket not found",
                    data: null
                ));
            }

            
            return Ok(new ResponseAPI<CustomerBasket>(
                statuscode: 200,
                message: "Basket retrieved successfully.",
                data: resultBasket
            ));
        }

       [HttpPost]
        public async Task<IActionResult> AddOrUpdate(CustomerBasket customerBasket)
        {
            var resultBasket = await _unitOfWork.CustomerBasketRepositry.AddOrUpdateBasketAsync(customerBasket);

            if (resultBasket == null)
            {
               
                return BadRequest(new ResponseAPI<object>(400, "Failed to add or update item."));
            }

            
            return Ok(new ResponseAPI<CustomerBasket>(200,  "operation successfully done.", resultBasket));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(String id) 
        {
        var resultBasket = await _unitOfWork.CustomerBasketRepositry.DeleteBasketAsync(id);
            if (resultBasket == false)
            {   
                return BadRequest(new ResponseAPI<object>(
                    statuscode: 400,
                    message: "failed to delete item."
                ));
            }

            return Ok(new ResponseAPI<object>(
                statuscode: 200,
                message: "item deleted"
            ));
        }
    }
}
