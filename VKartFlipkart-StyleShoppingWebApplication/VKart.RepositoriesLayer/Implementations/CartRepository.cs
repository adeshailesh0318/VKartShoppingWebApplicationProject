using Microsoft.EntityFrameworkCore;
using VKart.DataAccessLayer.Context;
using VKart.ModelsLayer.Entities;
using VKart.RepositoriesLayer.Interfaces;

namespace VKart.RepositoriesLayer.Implementations;

public class CartRepository : GenericRepository<Cart>, ICartRepository
{
    public CartRepository(VKartDbContext context) : base(context) { }

    public async Task<Cart?> GetByUserIdWithItemsAsync(string userId)
        => await _dbSet
            .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
            .FirstOrDefaultAsync(c => c.UserId == userId);

    public async Task<CartItem?> GetCartItemAsync(int cartId, int productId)
        => await _context.CartItems
            .FirstOrDefaultAsync(ci => ci.CartId == cartId && ci.ProductId == productId);

    public async Task<CartItem?> GetCartItemByIdAsync(int cartItemId)
        => await _context.CartItems
            .Include(ci => ci.Product)
            .FirstOrDefaultAsync(ci => ci.CartItemId == cartItemId);
}