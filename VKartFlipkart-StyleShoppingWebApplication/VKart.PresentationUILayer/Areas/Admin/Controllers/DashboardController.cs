using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VKart.ModelsLayer.DTOs;
using VKart.ModelsLayer.Entities;
using VKart.PresentationLayer.ViewModels;
using VKart.ServicesLayer.Interfaces;

namespace VKart.PresentationLayer.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class DashboardController : Controller
{
    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;
    private readonly IOrderService _orderService;
    private readonly UserManager<ApplicationUser> _userManager;

    public DashboardController(
        IProductService productService,
        ICategoryService categoryService,
        IOrderService orderService,
        UserManager<ApplicationUser> userManager)
    {
        _productService = productService;
        _categoryService = categoryService;
        _orderService = orderService;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        // Load data sequentially (one by one) to avoid EF Core DbContext concurrency issues
        var productsResult = await _productService.SearchAsync(new ProductQueryDto { Page = 1, PageSize = 1000 });
        var categoriesResult = await _categoryService.GetAllAsync();
        var ordersResult = await _orderService.GetAllOrdersAsync();

        var products = productsResult.Data?.ToList() ?? new List<ProductDto>();
        var categories = categoriesResult.Data?.ToList() ?? new List<CategoryDto>();
        var orders = ordersResult.Data?.ToList() ?? new List<OrderDto>();

        var vm = new AdminDashboardViewModel
        {
            TotalProducts = products.Count,
            TotalCategories = categories.Count,
            TotalOrders = orders.Count,
            TotalCustomers = _userManager.Users.Count(),
            TotalRevenue = orders.Where(o => o.OrderStatusText != "Cancelled").Sum(o => o.TotalAmount),
            RecentOrders = orders.OrderByDescending(o => o.OrderDate).Take(5).ToList(),
            LowStockProducts = products.Where(p => p.StockQuantity <= 10 && p.IsActive).OrderBy(p => p.StockQuantity).Take(5).ToList()
        };

        return View(vm);
    }
}