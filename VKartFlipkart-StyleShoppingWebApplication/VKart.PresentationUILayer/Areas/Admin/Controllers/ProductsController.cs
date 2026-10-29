using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using VKart.ModelsLayer.DTOs;
using VKart.ServicesLayer.Interfaces;

namespace VKart.PresentationLayer.Areas.Admin.Controllers;

[Area("Admin")]

[Authorize(Roles = "Admin")]
public class ProductsController : Controller
{
    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;
    private readonly IWebHostEnvironment _webHostEnvironment; // <-- ADD THIS LINE

    // Update the constructor to accept the new service
    public ProductsController(IProductService productService, ICategoryService categoryService, IWebHostEnvironment webHostEnvironment)
    {
        _productService = productService;
        _categoryService = categoryService;
        _webHostEnvironment = webHostEnvironment; // <-- ADD THIS LINE
    }
    public async Task<IActionResult> Index()
    {
        var result = await _productService.SearchAsync(new ProductQueryDto { Page = 1, PageSize = 500 });
        // Show ALL products including inactive (admin view)
        return View(result.Data ?? Enumerable.Empty<ProductDto>());
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await LoadCategories();
        return View(new CreateProductDto { IsActive = true });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateProductDto dto)
    {
        if (!ModelState.IsValid)
        {
            await LoadCategories();
            return View(dto);
        }

        // --- NEW: Handle Image Upload ---
        if (dto.ImageFile != null && dto.ImageFile.Length > 0)
        {
            // 1. Create a unique file name
            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.ImageFile.FileName);

            // 2. Define the path (e.g., wwwroot/images/products)
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "products");

            // 3. Create directory if it doesn't exist
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // 4. Save the file physically
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await dto.ImageFile.CopyToAsync(fileStream);
            }

            // 5. Set the ImageUrl in the DTO so the Service/Database saves the path
            dto.ImageUrl = "/images/products/" + uniqueFileName;
        }
        // ---------------------------------

        var result = await _productService.CreateAsync(dto);
        if (!result.Success)
        {
            ModelState.AddModelError("", result.Error ?? "Failed.");
            await LoadCategories();
            return View(dto);
        }

        TempData["Success"] = $"Product '{dto.ProductName}' created.";
        return RedirectToAction(nameof(Index));
    }


    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var result = await _productService.GetByIdAsync(id);
        if (!result.Success || result.Data == null) return NotFound();

        var p = result.Data;
        await LoadCategories();
        return View(new CreateProductDto
        {
            ProductName = p.ProductName,
            Description = p.Description,
            Price = p.Price,
            StockQuantity = p.StockQuantity,
            ImageUrl = p.ImageUrl,
            CategoryId = p.CategoryId,
            IsActive = p.IsActive
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CreateProductDto dto)
    {
        if (!ModelState.IsValid)
        {
            await LoadCategories();
            return View(dto);
        }

        var result = await _productService.UpdateAsync(id, dto);
        if (!result.Success)
        {
            ModelState.AddModelError("", result.Error ?? "Failed.");
            await LoadCategories();
            return View(dto);
        }

        TempData["Success"] = $"Product '{dto.ProductName}' updated.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _productService.DeleteAsync(id);
        if (!result.Success)
            TempData["Error"] = result.Error;
        else
            TempData["Success"] = "Product deleted (soft delete).";

        return RedirectToAction(nameof(Index));
    }

    private async Task LoadCategories()
    {
        var cats = await _categoryService.GetAllAsync();
        ViewBag.Categories = new SelectList(cats.Data ?? Enumerable.Empty<CategoryDto>(), "CategoryId", "CategoryName");
    }
}