using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VKart.ModelsLayer.DTOs;
using VKart.ServicesLayer.Interfaces;
using VKart.WebApiLayer.Extensions;

namespace VKart.WebApiLayer.Controllers;

[ApiController]
[Route("api/cart")]
[Authorize]
public class CartController : ControllerBase
{
    private readonly ICartService _cart;

    public CartController(ICartService cart) => _cart = cart;

    [HttpGet]
    public async Task<IActionResult> GetCart()
    {
        var result = await _cart.GetCartAsync(User.GetUserId());
        return Ok(result.Data);
    }

    [HttpPost("items")]
    public async Task<IActionResult> AddItem([FromBody] AddToCartDto dto)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        var result = await _cart.AddToCartAsync(User.GetUserId(), dto);
        if (!result.Success) return BadRequest(new { error = result.Error });
        return Ok(result.Data);
    }

    [HttpPut("items/{cartItemId:int}")]
    public async Task<IActionResult> UpdateQuantity(int cartItemId, [FromQuery] int quantity)
    {
        var result = await _cart.UpdateQuantityAsync(User.GetUserId(), cartItemId, quantity);
        if (!result.Success) return BadRequest(new { error = result.Error });
        return Ok(result.Data);
    }

    [HttpDelete("items/{cartItemId:int}")]
    public async Task<IActionResult> RemoveItem(int cartItemId)
    {
        var result = await _cart.RemoveItemAsync(User.GetUserId(), cartItemId);
        if (!result.Success) return NotFound(new { error = result.Error });
        return Ok(result.Data);
    }

    [HttpDelete]
    public async Task<IActionResult> Clear()
    {
        await _cart.ClearCartAsync(User.GetUserId());
        return NoContent();
    }
}