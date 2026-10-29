using VKart.ModelsLayer.Entities;

namespace VKart.RepositoriesLayer.Interfaces;

public interface ICategoryRepository : IGenericRepository<Category>
{
    Task<IEnumerable<Category>> GetAllWithProductCountAsync();
}