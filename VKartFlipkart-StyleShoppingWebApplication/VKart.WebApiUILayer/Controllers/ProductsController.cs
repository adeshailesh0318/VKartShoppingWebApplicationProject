using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VKart.ModelsLayer.DTOs;
using VKart.ServicesLayer.Interfaces;

namespace VKart.WebApiLayer.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _product;

    public ProductsController(IProductService product) => _product = product;

    /// <summary>
    /// Search/sort/filter/paginate.
    /// Example: GET /api/products?search=phone&categoryId=1&sortBy=price_asc&page=1&pageSize=10
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Search([FromQuery] ProductQueryDto query)
    {
        var result = await _product.SearchAsync(query);
        return Ok(result.Data);
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _product.GetByIdAsync(id);
        if (!result.Success) return NotFound(new { error = result.Error });
        return Ok(result.Data);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        var result = await _product.CreateAsync(dto);
        if (!result.Success) return BadRequest(new { error = result.Error });
        return CreatedAtAction(nameof(GetById), new { id = result.Data!.ProductId }, result.Data);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateProductDto dto)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        var result = await _product.UpdateAsync(id, dto);
        if (!result.Success) return NotFound(new { error = result.Error });
        return Ok(result.Data);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _product.DeleteAsync(id);
        if (!result.Success) return NotFound(new { error = result.Error });
        return NoContent();
    }
}