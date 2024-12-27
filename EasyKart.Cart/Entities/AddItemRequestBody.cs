using EasyKart.Shared.Models;

namespace EasyKart.Cart.Entities
{
    public class AddItemRequestBody
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }
    }
}
