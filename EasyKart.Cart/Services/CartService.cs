using EasyKart.Cart.Repositories;
using Models = EasyKart.Shared.Models;
using System.Reflection;
using EasyKart.Shared.Models;

namespace EasyKart.Cart.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private Guid tempUserId = Guid.Parse("d8e1c062-4d3e-4326-9f16-31b28f62a4c5");
        public CartService(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        public async Task<Models.Cart> AddItemToCartAsync(Models.Product product, int quantity)
        {

            Models.Cart cart = await GetCartAsync(tempUserId);
            if (cart == null)
            {
                cart = new Models.Cart(tempUserId);
                cart.addProduct(product, quantity);
            }
            else
            {
                cart.addProduct(product, quantity);
            }

            await _cartRepository.UpdateCartAsync(cart);
            cart = await GetCartAsync(tempUserId);

            return cart;
        }

        public async Task<Models.Cart> GetCartAsync(Guid userId)
        {
            Models.Cart cart = await _cartRepository.GetCartAsync(userId);
            if (cart == null)
            {
                cart = new Models.Cart(userId);
                await _cartRepository.AddItemToCartAsync(cart);
                cart = await _cartRepository.GetCartAsync(userId);
            }            
            return cart;
        }
    }
}
