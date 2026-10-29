using Microsoft.EntityFrameworkCore;
using VKart.ModelsLayer.DTOs;
using VKart.ModelsLayer.Entities;
using VKart.RepositoriesLayer.Interfaces;
using VKart.ServicesLayer.Common;
using VKart.ServicesLayer.Interfaces;

namespace VKart.ServicesLayer.Implementations;

public class ProductService : IProductService
{
    private readonly IProductRepository _repo;

    public ProductService(IProductRepository repo) => _repo = repo;

    public async Task<ServiceResult<IEnumerable<ProductDto>>> SearchAsync(ProductQueryDto q)
    {
        var query = _repo.Query().Include(p => p.Category).AsQueryable();

        // Filter: only active products for customer view (admin can still filter via IsActive later)
        query = query.Where(p => p.IsActive);

        if (!string.IsNullOrWhiteSpace(q.Search))
        {
            var s = q.Search.Trim().ToLower();
            query = query.Where(p => p.ProductName.ToLower().Contains(s));
        }

        if (q.CategoryId.HasValue && q.CategoryId.Value > 0)
            query = query.Where(p => p.CategoryId == q.CategoryId.Value);

        query = q.SortBy switch
        {
            "price_asc" => query.OrderBy(p => p.Price),
            "price_desc" => query.OrderByDescending(p => p.Price),
            "name_asc" => query.OrderBy(p => p.ProductName),
            "name_desc" => query.OrderByDescending(p => p.ProductName),
            _ => query.OrderByDescending(p => p.CreatedDate) // newest
        };

        var page = q.Page < 1 ? 1 : q.Page;
        var size = q.PageSize < 1 ? 20 : q.PageSize;
        query = query.Skip((page - 1) * size).Take(size);

        var products = await query.AsNoTracking().ToListAsync();

        return ServiceResult<IEnumerable<ProductDto>>.Ok(products.Select(Map));
    }

    public async Task<ServiceResult<ProductDto>> GetByIdAsync(int id)
    {
        var p = await _repo.GetWithCategoryAsync(id);
        if (p is null) return ServiceResult<ProductDto>.Fail("Product not found.");
        return ServiceResult<ProductDto>.Ok(Map(p));
    }

    public async Task<ServiceResult<ProductDto>> CreateAsync(CreateProductDto dto)
    {
        var product = new Product
        {
            ProductName = dto.ProductName.Trim(),
            Description = dto.Description,
            Price = dto.Price,
            StockQuantity = dto.StockQuantity,
            ImageUrl = dto.ImageUrl,
            CategoryId = dto.CategoryId,
            IsActive = dto.IsActive
        };
        await _repo.AddAsync(product);
        await _repo.SaveChangesAsync();

        // reload with category name
        var reloaded = await _repo.GetWithCategoryAsync(product.ProductId);
        return ServiceResult<ProductDto>.Ok(Map(reloaded!));
    }

    public async Task<ServiceResult<ProductDto>> UpdateAsync(int id, CreateProductDto dto)
    {
        var product = await _repo.GetByIdAsync(id);
        if (product is null) return ServiceResult<ProductDto>.Fail("Product not found.");

        product.ProductName = dto.ProductName.Trim();
        product.Description = dto.Description;
        product.Price = dto.Price;
        product.StockQuantity = dto.StockQuantity;
        product.ImageUrl = dto.ImageUrl;
        product.CategoryId = dto.CategoryId;
        product.IsActive = dto.IsActive;

        _repo.Update(product);
        await _repo.SaveChangesAsync();

        var reloaded = await _repo.GetWithCategoryAsync(id);
        return ServiceResult<ProductDto>.Ok(Map(reloaded!));
    }

    public async Task<ServiceResult> DeleteAsync(int id)
    {
        var product = await _repo.GetByIdAsync(id);
        if (product is null) return ServiceResult.Fail("Product not found.");

        // Soft delete — keep order history intact
        product.IsActive = false;
        _repo.Update(product);
        await _repo.SaveChangesAsync();
        return ServiceResult.Ok();
    }

    private static ProductDto Map(Product p) => new()
    {
        ProductId = p.ProductId,
        ProductName = p.ProductName,
        Description = p.Description,
        Price = p.Price,
        StockQuantity = p.StockQuantity,
        ImageUrl = p.ImageUrl,
        CategoryId = p.CategoryId,
        CategoryName = p.Category?.CategoryName,
        CreatedDate = p.CreatedDate,
        IsActive = p.IsActive
    };
}