using EasyKart.Cart.Repositories;
using Models = EasyKart.Shared.Models;
using System.Reflection;
using EasyKart.Shared.Models;
using MassTransit.Testing;
using MassTransit;
using MassTransit.Transports;

namespace EasyKart.Cart.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private Guid tempUserId = Guid.Parse("d8e1c062-4d3e-4326-9f16-31b28f62a4c5");
        private IPublishEndpoint publishEndpoint;
        public CartService(ICartRepository cartRepository, IPublishEndpoint publishEndpoint)
        {
            _cartRepository = cartRepository;
            this.publishEndpoint = publishEndpoint;
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

            Notification notification = new Notification() 
            {
                Id = Guid.NewGuid(),
                Message = "Cart updated",
                Title ="Cart Updated",
                Type = "CartUpdate",
                UserId = Guid.Parse("d8e1c062-4d3e-4326-9f16-31b28f62a4c5")
            };
            await publishEndpoint.Publish(notification);
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

        public async Task<bool> EmptyCartAsync(Guid userId)
        {
            bool res = await _cartRepository.EmptyCartAsync(userId);
            if (res)
            {
                Notification notification = new Notification()
                {
                    Id = Guid.NewGuid(),
                    Message = "Cart updated",
                    Title = "Cart Updated",
                    Type = "CartEmpty",
                    UserId = Guid.Parse("d8e1c062-4d3e-4326-9f16-31b28f62a4c5")
                };
                await publishEndpoint.Publish(notification);
            }
            return res;
        }
    }
}
