using Microsoft.EntityFrameworkCore;
using VKart.DataAccessLayer.Context;
using VKart.ModelsLayer.Entities;
using VKart.RepositoriesLayer.Interfaces;

namespace VKart.RepositoriesLayer.Implementations;

public class ProductRepository : GenericRepository<Product>, IProductRepository
{
    public ProductRepository(VKartDbContext context) : base(context) { }

    public async Task<Product?> GetWithCategoryAsync(int id)
        => await _dbSet.Include(p => p.Category).FirstOrDefaultAsync(p => p.ProductId == id);

    public async Task<IEnumerable<Product>> GetActiveWithCategoryAsync()
        => await _dbSet.Include(p => p.Category).Where(p => p.IsActive).AsNoTracking().ToListAsync();
}