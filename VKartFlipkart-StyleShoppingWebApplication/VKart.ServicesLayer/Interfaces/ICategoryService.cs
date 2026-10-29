using VKart.ModelsLayer.DTOs;
using VKart.ServicesLayer.Common;

namespace VKart.ServicesLayer.Interfaces;

public interface ICategoryService
{
    Task<ServiceResult<IEnumerable<CategoryDto>>> GetAllAsync();
    Task<ServiceResult<CategoryDto>> GetByIdAsync(int id);
    Task<ServiceResult<CategoryDto>> CreateAsync(CreateCategoryDto dto);
    Task<ServiceResult<CategoryDto>> UpdateAsync(int id, CreateCategoryDto dto);
    Task<ServiceResult> DeleteAsync(int id);
}