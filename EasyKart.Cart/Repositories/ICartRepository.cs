namespace EasyKart.Cart.Repositories
{
    public interface ICartRepository
    {
        Task<bool> AddItemToCartAsync(EasyKart.Shared.Models.Cart cart);
        Task<EasyKart.Shared.Models.Cart> GetCartAsync(Guid userId);
        Task<bool> UpdateCartAsync(EasyKart.Shared.Models.Cart cart);
        Task<bool> EmptyCartAsync(Guid userId);
    }
}
