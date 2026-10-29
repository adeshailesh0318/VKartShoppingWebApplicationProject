using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VKart.ModelsLayer.DTOs;
using VKart.ModelsLayer.Enums;
using VKart.ServicesLayer.Interfaces;
using VKart.WebApiLayer.Extensions;

namespace VKart.WebApiLayer.Controllers;

[ApiController]
[Route("api/orders")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _order;

    public OrdersController(IOrderService order) => _order = order;

    /// <summary>Place an order from the current cart (Cash on Delivery).</summary>
    [HttpPost]
    public async Task<IActionResult> PlaceOrder([FromBody] PlaceOrderDto dto)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        var result = await _order.PlaceOrderAsync(User.GetUserId(), dto);
        if (!result.Success) return BadRequest(new { error = result.Error });
        return Ok(result.Data);
    }

    /// <summary>My orders (customer).</summary>
    [HttpGet("my")]
    public async Task<IActionResult> MyOrders()
    {
        var result = await _order.GetMyOrdersAsync(User.GetUserId());
        return Ok(result.Data);
    }

    /// <summary>Get a specific order — customer can access only their own; admin any.</summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _order.GetOrderAsync(User.GetUserId(), id, User.IsAdmin());
        if (!result.Success) return NotFound(new { error = result.Error });
        return Ok(result.Data);
    }

    /// <summary>Admin: list all orders.</summary>
    [HttpGet("admin/all")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _order.GetAllOrdersAsync();
        return Ok(result.Data);
    }

    /// <summary>Admin: update order status.</summary>
    [HttpPut("admin/{id:int}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateStatus(int id, [FromQuery] OrderStatus status)
    {
        var result = await _order.UpdateStatusAsync(id, status);
        if (!result.Success) return NotFound(new { error = result.Error });
        return Ok(result.Data);
    }
}