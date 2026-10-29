using VKart.ModelsLayer.DTOs;
using VKart.ServicesLayer.Common;

namespace VKart.ServicesLayer.Interfaces;

public interface ICartService
{
    Task<ServiceResult<CartDto>> GetCartAsync(string userId);
    Task<ServiceResult<CartDto>> AddToCartAsync(string userId, AddToCartDto dto);
    Task<ServiceResult<CartDto>> UpdateQuantityAsync(string userId, int cartItemId, int quantity);
    Task<ServiceResult<CartDto>> RemoveItemAsync(string userId, int cartItemId);
    Task<ServiceResult> ClearCartAsync(string userId);
}