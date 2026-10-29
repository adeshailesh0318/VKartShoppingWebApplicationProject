using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VKart.ModelsLayer.Entities;
using VKart.ServicesLayer.Interfaces;
using VKart.PresentationLayer.ViewModels;

namespace VKart.PresentationLayer.Controllers;

[Authorize]
public class OrdersController : Controller
{
    private readonly IOrderService _orderService;
    private readonly ICartService _cartService;
    private readonly UserManager<ApplicationUser> _userManager;

    public OrdersController(
        IOrderService orderService,
        ICartService cartService,
        UserManager<ApplicationUser> userManager)
    {
        _orderService = orderService;
        _cartService = cartService;
        _userManager = userManager;
    }

    private string CurrentUserId => _userManager.GetUserId(User)!;

    // GET: /Orders/Checkout
    public async Task<IActionResult> Checkout()
    {
        var cartResult = await _cartService.GetCartAsync(CurrentUserId);
        if (cartResult.Data == null || !cartResult.Data.Items.Any())
        {
            TempData["Error"] = "Your cart is empty.";
            return RedirectToAction("Index", "Cart");
        }

        return View(new CheckoutViewModel { Cart = cartResult.Data });
    }

    // POST: /Orders/Checkout
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Checkout(CheckoutViewModel vm)
    {
        // Reload cart (in case it changed)
        var cartResult = await _cartService.GetCartAsync(CurrentUserId);
        vm.Cart = cartResult.Data!;

        if (!ModelState.IsValid)
            return View(vm);

        if (!vm.Cart.Items.Any())
        {
            ModelState.AddModelError("", "Your cart is empty.");
            return View(vm);
        }

        var orderResult = await _orderService.PlaceOrderAsync(CurrentUserId, new()
        {
            ShippingAddress = vm.ShippingAddress
        });

        if (!orderResult.Success || orderResult.Data == null)
        {
            ModelState.AddModelError("", orderResult.Error ?? "Failed to place order.");
            return View(vm);
        }

        TempData["Success"] = $"Order #{orderResult.Data.OrderId} placed successfully!";
        return RedirectToAction(nameof(Confirmation), new { id = orderResult.Data.OrderId });
    }

    // GET: /Orders/Confirmation/5
    public async Task<IActionResult> Confirmation(int id)
    {
        var result = await _orderService.GetOrderAsync(CurrentUserId, id, isAdmin: false);
        if (!result.Success || result.Data == null) return NotFound();

        return View(result.Data);
    }

    // GET: /Orders (My Orders)
    public async Task<IActionResult> Index()
    {
        var result = await _orderService.GetMyOrdersAsync(CurrentUserId);
        return View(result.Data ?? Enumerable.Empty<VKart.ModelsLayer.DTOs.OrderDto>());
    }

    // GET: /Orders/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var result = await _orderService.GetOrderAsync(CurrentUserId, id, isAdmin: false);
        if (!result.Success || result.Data == null) return NotFound();

        return View(result.Data);
    }
}