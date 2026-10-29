using VKart.ModelsLayer.DTOs;
using VKart.ServicesLayer.Common;

namespace VKart.ServicesLayer.Interfaces;

public interface IProductService
{
    Task<ServiceResult<IEnumerable<ProductDto>>> SearchAsync(ProductQueryDto query);
    Task<ServiceResult<ProductDto>> GetByIdAsync(int id);
    Task<ServiceResult<ProductDto>> CreateAsync(CreateProductDto dto);
    Task<ServiceResult<ProductDto>> UpdateAsync(int id, CreateProductDto dto);
    Task<ServiceResult> DeleteAsync(int id);
}