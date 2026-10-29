using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VKart.ModelsLayer.DTOs;
using VKart.ServicesLayer.Interfaces;

namespace VKart.PresentationLayer.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class CategoriesController : Controller
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService) => _categoryService = categoryService;

    public async Task<IActionResult> Index()
    {
        var result = await _categoryService.GetAllAsync();
        return View(result.Data ?? Enumerable.Empty<CategoryDto>());
    }

    [HttpGet]
    public IActionResult Create() => View("Form", new CreateCategoryDto());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateCategoryDto dto)
    {
        if (!ModelState.IsValid) return View("Form", dto);

        var result = await _categoryService.CreateAsync(dto);
        if (!result.Success)
        {
            ModelState.AddModelError("", result.Error ?? "Failed.");
            return View("Form", dto);
        }

        TempData["Success"] = $"Category '{dto.CategoryName}' created.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var result = await _categoryService.GetByIdAsync(id);
        if (!result.Success || result.Data == null) return NotFound();

        return View("Form", new CreateCategoryDto
        {
            CategoryName = result.Data.CategoryName,
            Description = result.Data.Description
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CreateCategoryDto dto)
    {
        if (!ModelState.IsValid) return View("Form", dto);

        var result = await _categoryService.UpdateAsync(id, dto);
        if (!result.Success)
        {
            ModelState.AddModelError("", result.Error ?? "Failed.");
            return View("Form", dto);
        }

        TempData["Success"] = $"Category '{dto.CategoryName}' updated.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _categoryService.DeleteAsync(id);
        if (!result.Success)
            TempData["Error"] = result.Error;
        else
            TempData["Success"] = "Category deleted.";

        return RedirectToAction(nameof(Index));
    }
}