using VKart.ModelsLayer.Entities;

namespace VKart.RepositoriesLayer.Interfaces;

public interface ICartRepository : IGenericRepository<Cart>
{
    Task<Cart?> GetByUserIdWithItemsAsync(string userId);
    Task<CartItem?> GetCartItemAsync(int cartId, int productId);
    Task<CartItem?> GetCartItemByIdAsync(int cartItemId);
}