using VKart.ModelsLayer.DTOs;
using VKart.ModelsLayer.Entities;
using VKart.RepositoriesLayer.Interfaces;
using VKart.ServicesLayer.Common;
using VKart.ServicesLayer.Interfaces;

namespace VKart.ServicesLayer.Implementations;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _repo;
    public CategoryService(ICategoryRepository repo) => _repo = repo;

    public async Task<ServiceResult<IEnumerable<CategoryDto>>> GetAllAsync()
    {
        var cats = await _repo.GetAllWithProductCountAsync();
        var dtos = cats.Select(c => new CategoryDto
        {
            CategoryId = c.CategoryId,
            CategoryName = c.CategoryName,
            Description = c.Description,
            ProductCount = c.Products.Count
        });
        return ServiceResult<IEnumerable<CategoryDto>>.Ok(dtos);
    }

    public async Task<ServiceResult<CategoryDto>> GetByIdAsync(int id)
    {
        var c = await _repo.GetByIdAsync(id);
        if (c is null) return ServiceResult<CategoryDto>.Fail("Category not found.");
        return ServiceResult<CategoryDto>.Ok(new CategoryDto
        {
            CategoryId = c.CategoryId,
            CategoryName = c.CategoryName,
            Description = c.Description
        });
    }

    public async Task<ServiceResult<CategoryDto>> CreateAsync(CreateCategoryDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.CategoryName))
            return ServiceResult<CategoryDto>.Fail("Category name is required.");

        var category = new Category
        {
            CategoryName = dto.CategoryName.Trim(),
            Description = dto.Description
        };
        await _repo.AddAsync(category);
        await _repo.SaveChangesAsync();

        return ServiceResult<CategoryDto>.Ok(new CategoryDto
        {
            CategoryId = category.CategoryId,
            CategoryName = category.CategoryName,
            Description = category.Description
        });
    }

    public async Task<ServiceResult<CategoryDto>> UpdateAsync(int id, CreateCategoryDto dto)
    {
        var c = await _repo.GetByIdAsync(id);
        if (c is null) return ServiceResult<CategoryDto>.Fail("Category not found.");

        c.CategoryName = dto.CategoryName.Trim();
        c.Description = dto.Description;
        _repo.Update(c);
        await _repo.SaveChangesAsync();

        return ServiceResult<CategoryDto>.Ok(new CategoryDto
        {
            CategoryId = c.CategoryId,
            CategoryName = c.CategoryName,
            Description = c.Description
        });
    }

    public async Task<ServiceResult> DeleteAsync(int id)
    {
        var c = await _repo.GetByIdAsync(id);
        if (c is null) return ServiceResult.Fail("Category not found.");

        _repo.Remove(c);
        await _repo.SaveChangesAsync();
        return ServiceResult.Ok();
    }
}