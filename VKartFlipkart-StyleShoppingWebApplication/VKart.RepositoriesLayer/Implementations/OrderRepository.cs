using Microsoft.EntityFrameworkCore;
using VKart.DataAccessLayer.Context;
using VKart.ModelsLayer.Entities;
using VKart.RepositoriesLayer.Interfaces;

namespace VKart.RepositoriesLayer.Implementations;

public class OrderRepository : GenericRepository<Order>, IOrderRepository
{
    public OrderRepository(VKartDbContext context) : base(context) { }

    public async Task<IEnumerable<Order>> GetByUserIdAsync(string userId)
        => await _dbSet
            .Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.OrderDate)
            .AsNoTracking()
            .ToListAsync();

    public async Task<Order?> GetWithItemsAsync(int orderId)
        => await _dbSet
            .Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
            .Include(o => o.User)
            .FirstOrDefaultAsync(o => o.OrderId == orderId);

    public async Task<IEnumerable<Order>> GetAllWithItemsAsync()
        => await _dbSet
            .Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
            .Include(o => o.User)
            .OrderByDescending(o => o.OrderDate)
            .AsNoTracking()
            .ToListAsync();
}