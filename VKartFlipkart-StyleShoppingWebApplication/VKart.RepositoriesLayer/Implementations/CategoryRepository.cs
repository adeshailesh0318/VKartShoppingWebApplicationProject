using Microsoft.EntityFrameworkCore;
using VKart.DataAccessLayer.Context;
using VKart.ModelsLayer.Entities;
using VKart.RepositoriesLayer.Interfaces;

namespace VKart.RepositoriesLayer.Implementations;

public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
{
    public CategoryRepository(VKartDbContext context) : base(context) { }

    public async Task<IEnumerable<Category>> GetAllWithProductCountAsync()
        => await _dbSet.Include(c => c.Products).AsNoTracking().ToListAsync();
}