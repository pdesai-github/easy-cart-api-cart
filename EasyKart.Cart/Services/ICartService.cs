using EasyKart.Cart.Repositories;
using EKModels=EasyKart.Shared.Models;

namespace EasyKart.Cart.Services
{
    public interface ICartService
    {
        Task<EKModels.Cart> AddItemToCartAsync(EKModels.Product product, int quantity);
        Task<EKModels.Cart> GetCartAsync(Guid userId);
    }
}
