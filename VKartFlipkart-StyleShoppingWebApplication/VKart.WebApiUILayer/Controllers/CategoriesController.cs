using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VKart.ModelsLayer.DTOs;
using VKart.ServicesLayer.Interfaces;

namespace VKart.WebApiLayer.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _category;

    public CategoriesController(ICategoryService category) => _category = category;

    /// <summary>List all categories (public).</summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        var result = await _category.GetAllAsync();
        return Ok(result.Data);
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _category.GetByIdAsync(id);
        if (!result.Success) return NotFound(new { error = result.Error });
        return Ok(result.Data);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateCategoryDto dto)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        var result = await _category.CreateAsync(dto);
        if (!result.Success) return BadRequest(new { error = result.Error });
        return CreatedAtAction(nameof(GetById), new { id = result.Data!.CategoryId }, result.Data);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateCategoryDto dto)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        var result = await _category.UpdateAsync(id, dto);
        if (!result.Success) return NotFound(new { error = result.Error });
        return Ok(result.Data);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _category.DeleteAsync(id);
        if (!result.Success) return NotFound(new { error = result.Error });
        return NoContent();
    }
}