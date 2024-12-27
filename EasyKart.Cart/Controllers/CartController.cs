using EasyKart.Cart.Repositories;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using EKModels = EasyKart.Shared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EasyKart.Cart.Services;
using EasyKart.Cart.Entities;

namespace EasyKart.Cart.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpPost("AddItemToCartAsync")]
        public async Task<IActionResult> AddItemToCartAsync([FromBody]AddItemRequestBody addItemRequestBody)
        {
            try
            {
                EKModels.Cart result = await _cartService.AddItemToCartAsync(addItemRequestBody.Product, addItemRequestBody.Quantity);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetCartAsync(Guid userId)
        {
            try
            {
                var result = await _cartService.GetCartAsync(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
