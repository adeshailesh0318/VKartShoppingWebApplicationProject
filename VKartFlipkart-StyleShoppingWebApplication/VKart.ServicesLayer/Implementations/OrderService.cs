using VKart.DataAccessLayer.Context;
using VKart.ModelsLayer.DTOs;
using VKart.ModelsLayer.Entities;
using VKart.ModelsLayer.Enums;
using VKart.RepositoriesLayer.Interfaces;
using VKart.ServicesLayer.Common;
using VKart.ServicesLayer.Interfaces;

namespace VKart.ServicesLayer.Implementations;

public class OrderService : IOrderService
{
    private readonly ICartRepository _cartRepo;
    private readonly IOrderRepository _orderRepo;
    private readonly IProductRepository _productRepo;
    private readonly VKartDbContext _context;

    public OrderService(
        ICartRepository cartRepo,
        IOrderRepository orderRepo,
        IProductRepository productRepo,
        VKartDbContext context)
    {
        _cartRepo = cartRepo;
        _orderRepo = orderRepo;
        _productRepo = productRepo;
        _context = context;
    }

    public async Task<ServiceResult<OrderDto>> PlaceOrderAsync(string userId, PlaceOrderDto dto)
    {
        var cart = await _cartRepo.GetByUserIdWithItemsAsync(userId);
        if (cart is null || !cart.CartItems.Any())
            return ServiceResult<OrderDto>.Fail("Cart is empty.");

        // Re-validate stock for every line
        foreach (var item in cart.CartItems)
        {
            if (item.Product is null || !item.Product.IsActive)
                return ServiceResult<OrderDto>.Fail($"Product unavailable.");
            if (item.Quantity > item.Product.StockQuantity)
                return ServiceResult<OrderDto>.Fail(
                    $"Only {item.Product.StockQuantity} of '{item.Product.ProductName}' available.");
        }

        // Begin transaction (order + stock + cart clear must be atomic)
        await using var tx = await _context.Database.BeginTransactionAsync();
        try
        {
            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                ShippingAddress = dto.ShippingAddress.Trim(),
                OrderStatus = OrderStatus.Pending,
                TotalAmount = cart.CartItems.Sum(ci => (ci.Product?.Price ?? 0) * ci.Quantity),
                OrderItems = cart.CartItems.Select(ci => new OrderItem
                {
                    ProductId = ci.ProductId,
                    Quantity = ci.Quantity,
                    UnitPrice = ci.Product?.Price ?? 0
                }).ToList()
            };

            // Deduct stock
            foreach (var ci in cart.CartItems)
            {
                ci.Product!.StockQuantity -= ci.Quantity;
            }

            // Clear cart
            cart.CartItems.Clear();

            await _orderRepo.AddAsync(order);
            await _context.SaveChangesAsync();
            await tx.CommitAsync();

            var reloaded = await _orderRepo.GetWithItemsAsync(order.OrderId);
            return ServiceResult<OrderDto>.Ok(Map(reloaded!));
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync();
            return ServiceResult<OrderDto>.Fail($"Order failed: {ex.Message}");
        }
    }

    public async Task<ServiceResult<IEnumerable<OrderDto>>> GetMyOrdersAsync(string userId)
    {
        var orders = await _orderRepo.GetByUserIdAsync(userId);
        return ServiceResult<IEnumerable<OrderDto>>.Ok(orders.Select(Map));
    }

    public async Task<ServiceResult<OrderDto>> GetOrderAsync(string userId, int orderId, bool isAdmin)
    {
        var order = await _orderRepo.GetWithItemsAsync(orderId);
        if (order is null) return ServiceResult<OrderDto>.Fail("Order not found.");

        if (!isAdmin && order.UserId != userId)
            return ServiceResult<OrderDto>.Fail("Access denied.");

        return ServiceResult<OrderDto>.Ok(Map(order));
    }

    public async Task<ServiceResult<IEnumerable<OrderDto>>> GetAllOrdersAsync()
    {
        var orders = await _orderRepo.GetAllWithItemsAsync();
        return ServiceResult<IEnumerable<OrderDto>>.Ok(orders.Select(Map));
    }

    public async Task<ServiceResult<OrderDto>> UpdateStatusAsync(int orderId, OrderStatus newStatus)
    {
        var order = await _orderRepo.GetWithItemsAsync(orderId);
        if (order is null) return ServiceResult<OrderDto>.Fail("Order not found.");

        order.OrderStatus = newStatus;
        _orderRepo.Update(order);
        await _orderRepo.SaveChangesAsync();

        var reloaded = await _orderRepo.GetWithItemsAsync(orderId);
        return ServiceResult<OrderDto>.Ok(Map(reloaded!));
    }

    private static OrderDto Map(Order o) => new()
    {
        OrderId = o.OrderId,
        OrderDate = o.OrderDate,
        TotalAmount = o.TotalAmount,
        OrderStatus = o.OrderStatus,
        ShippingAddress = o.ShippingAddress,
        CustomerName = o.User?.FullName ?? "",
        CustomerEmail = o.User?.Email ?? "",
        Items = o.OrderItems.Select(oi => new OrderItemDto
        {
            OrderItemId = oi.OrderItemId,
            ProductId = oi.ProductId,
            ProductName = oi.Product?.ProductName ?? "",
            ImageUrl = oi.Product?.ImageUrl, // <--- ADDED IT HERE
            Quantity = oi.Quantity,
            UnitPrice = oi.UnitPrice,
            LineTotal = oi.UnitPrice * oi.Quantity
        }).ToList()
    };
}