using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using VKart.DataAccessLayer.Context;
using VKart.ModelsLayer.DTOs;
using VKart.PresentationLayer.ViewModels;
using VKart.ServicesLayer.Interfaces;

namespace VKart.PresentationLayer.Controllers;

public class HomeController : Controller
{
    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;
    private readonly VKartDbContext _context;

    public HomeController(IProductService productService, ICategoryService categoryService, VKartDbContext context)
    {
        _productService = productService;
        _categoryService = categoryService;
        _context = context;
    }


    public async Task<IActionResult> Index(int page = 1)
    {
        int pageSize = 8; // Set to 8 as you requested earlier

        // 1. Get the current page of products from your service
        var products = await _productService.SearchAsync(new ProductQueryDto
        {
            Page = page,
            PageSize = pageSize,
            SortBy = "newest"
        });

        var categories = await _categoryService.GetAllAsync();

        // 2. Get the TOTAL count of all products directly from the database
        // (Replace 'AppDbContext' and '_context.Products' with your actual names)
        var totalCount = await _context.Products.CountAsync();

        // 3. Calculate total pages properly
        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

        // 4. Pass pagination data to the View
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = totalPages == 0 ? 1 : totalPages; // Ensure it's at least 1

        return View(new HomeViewModel
        {
            Products = products.Data ?? Enumerable.Empty<ProductDto>(),
            Categories = categories.Data ?? Enumerable.Empty<CategoryDto>()
        });
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => View(new { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
}