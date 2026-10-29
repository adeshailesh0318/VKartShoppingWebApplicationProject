using VKart.ModelsLayer.DTOs;
using VKart.ModelsLayer.Enums;
using VKart.ServicesLayer.Common;

namespace VKart.ServicesLayer.Interfaces;

public interface IOrderService
{
    Task<ServiceResult<OrderDto>> PlaceOrderAsync(string userId, PlaceOrderDto dto);
    Task<ServiceResult<IEnumerable<OrderDto>>> GetMyOrdersAsync(string userId);
    Task<ServiceResult<OrderDto>> GetOrderAsync(string userId, int orderId, bool isAdmin);
    Task<ServiceResult<IEnumerable<OrderDto>>> GetAllOrdersAsync();
    Task<ServiceResult<OrderDto>> UpdateStatusAsync(int orderId, OrderStatus newStatus);
}