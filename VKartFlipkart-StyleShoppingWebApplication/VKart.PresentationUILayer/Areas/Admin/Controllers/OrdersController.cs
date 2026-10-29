using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using VKart.ModelsLayer.DTOs;
using VKart.ModelsLayer.Enums;
using VKart.ServicesLayer.Interfaces;

namespace VKart.PresentationLayer.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class OrdersController : Controller
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService) => _orderService = orderService;

    public async Task<IActionResult> Index()
    {
        var result = await _orderService.GetAllOrdersAsync();
        return View(result.Data ?? Enumerable.Empty<OrderDto>());
    }

    public async Task<IActionResult> Details(int id)
    {
        var result = await _orderService.GetOrderAsync("", id, isAdmin: true);
        if (!result.Success || result.Data == null) return NotFound();
        return View(result.Data);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int id, OrderStatus status)
    {
        var result = await _orderService.UpdateStatusAsync(id, status);
        if (!result.Success)
            TempData["Error"] = result.Error;
        else
            TempData["Success"] = $"Order #{id} updated to {status}.";

        return RedirectToAction(nameof(Details), new { id });
    }
}