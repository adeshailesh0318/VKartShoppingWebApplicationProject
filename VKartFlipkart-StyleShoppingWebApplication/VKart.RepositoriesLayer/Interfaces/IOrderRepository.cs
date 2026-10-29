using VKart.ModelsLayer.Entities;

namespace VKart.RepositoriesLayer.Interfaces;

public interface IOrderRepository : IGenericRepository<Order>
{
    Task<IEnumerable<Order>> GetByUserIdAsync(string userId);
    Task<Order?> GetWithItemsAsync(int orderId);
    Task<IEnumerable<Order>> GetAllWithItemsAsync();
}