using VKart.ModelsLayer.DTOs;

namespace VKart.PresentationLayer.ViewModels;

public class AdminDashboardViewModel
{
    public int TotalProducts { get; set; }
    public int TotalCategories { get; set; }
    public int TotalOrders { get; set; }
    public int TotalCustomers { get; set; }
    public decimal TotalRevenue { get; set; }
    public IEnumerable<OrderDto> RecentOrders { get; set; } = new List<OrderDto>();
    public IEnumerable<ProductDto> LowStockProducts { get; set; } = new List<ProductDto>();
}