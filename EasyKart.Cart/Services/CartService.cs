using EasyKart.Cart.Repositories;
using Models = EasyKart.Shared.Models;
using System.Reflection;
using EasyKart.Shared.Models;

namespace EasyKart.Cart.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        public CartService(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        public async Task<Models.Cart> AddItemToCartAsync(Models.Product product, int quantity)
        {

            Models.Cart cart = await GetCartAsync(Guid.Parse("d8e1c062-4d3e-4326-9f16-31b28f62a4c5"));
            if (cart == null)
            {
                cart = new Models.Cart
                {
                    UserId = Guid.Parse("d8e1c062-4d3e-4326-9f16-31b28f62a4c5"),
                    Items = new List<Models.CartItem>()
                };
                cart.Items.Add(new Models.CartItem
                {
                    Product = product,
                    Quantity = quantity
                });
            }
            else
            {
                var item = cart.Items.FirstOrDefault(i => i.Product.Id == product.Id);
                if (item == null)
                {
                    cart.Items.Add(new Models.CartItem
                    {
                        Product = product,
                        Quantity = quantity
                    });
                }
                else
                {
                    item.Quantity += quantity;
                }
            }
            await _cartRepository.UpdateCartAsync(cart);
            cart = await GetCartAsync(Guid.Parse("d8e1c062-4d3e-4326-9f16-31b28f62a4c5"));

            CalculateCartPrice(cart);

            return cart;
        }

        private static void CalculateCartPrice(Models.Cart cart)
        {
            cart.Price = 0;
            foreach (var item in cart.Items)
            {
                cart.Price = cart.Price + (item.Product.Price * item.Quantity);
            }
        }

        public async Task<Models.Cart> GetCartAsync(Guid userId)
        {
            Models.Cart cart = await _cartRepository.GetCartAsync(userId);
            CalculateCartPrice(cart);
            return cart;
        }
    }
}
