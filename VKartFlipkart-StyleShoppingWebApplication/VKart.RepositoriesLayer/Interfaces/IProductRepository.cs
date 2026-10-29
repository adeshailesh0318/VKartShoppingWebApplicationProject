using VKart.ModelsLayer.Entities;

namespace VKart.RepositoriesLayer.Interfaces;

public interface IProductRepository : IGenericRepository<Product>
{
    Task<Product?> GetWithCategoryAsync(int id);
    Task<IEnumerable<Product>> GetActiveWithCategoryAsync();
}