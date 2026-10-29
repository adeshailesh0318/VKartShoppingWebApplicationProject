using Microsoft.AspNetCore.Mvc;
using VKart.ModelsLayer.DTOs;
using VKart.ServicesLayer.Interfaces;
using VKart.PresentationLayer.ViewModels;

namespace VKart.PresentationLayer.Controllers;

public class ProductsController : Controller
{
    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;

    public ProductsController(IProductService productService, ICategoryService categoryService)
    {
        _productService = productService;
        _categoryService = categoryService;
    }

    // GET: /Products?search=phone&categoryId=1&sortBy=price_asc
    public async Task<IActionResult> Index(string? search, int? categoryId, string? sortBy)
    {
        var query = new ProductQueryDto
        {
            Search = search,
            CategoryId = categoryId,
            SortBy = sortBy,
            Page = 1,
            PageSize = 50 // Load all for now
        };

        var productsResult = await _productService.SearchAsync(query);
        var categoriesResult = await _categoryService.GetAllAsync();

        var vm = new HomeViewModel
        {
            Products = productsResult.Data ?? Enumerable.Empty<ProductDto>(),
            Categories = categoriesResult.Data ?? Enumerable.Empty<CategoryDto>(),
            Search = search,
            CategoryId = categoryId,
            SortBy = sortBy
        };

        return View(vm);
    }

    // GET: /Products/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var result = await _productService.GetByIdAsync(id);

        if (!result.Success || result.Data == null)
        {
            return NotFound(); // 404
        }

        return View(new ProductDetailsViewModel { Product = result.Data });
    }
}