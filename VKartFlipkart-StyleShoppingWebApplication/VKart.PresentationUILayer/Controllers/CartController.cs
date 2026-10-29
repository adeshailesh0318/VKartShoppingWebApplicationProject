using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VKart.ModelsLayer.Entities;
using VKart.ServicesLayer.Interfaces;
using VKart.PresentationLayer.ViewModels;

namespace VKart.PresentationLayer.Controllers;

[Authorize] // Must be logged in to access cart
public class CartController : Controller
{
    private readonly ICartService _cartService;
    private readonly UserManager<ApplicationUser> _userManager;

    public CartController(ICartService cartService, UserManager<ApplicationUser> userManager)
    {
        _cartService = cartService;
        _userManager = userManager;
    }

    private string CurrentUserId => _userManager.GetUserId(User)!;

    // GET: /Cart
    public async Task<IActionResult> Index()
    {
        var result = await _cartService.GetCartAsync(CurrentUserId);
        return View(new CartViewModel { Cart = result.Data! });
    }

    // POST: /Cart/Add
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(int productId, int quantity)
    {
        if (quantity < 1) quantity = 1;

        var result = await _cartService.AddToCartAsync(CurrentUserId, new()
        {
            ProductId = productId,
            Quantity = quantity
        });

        if (!result.Success)
        {
            TempData["Error"] = result.Error;
            return RedirectToAction("Details", "Products", new { id = productId });
        }

        TempData["Success"] = "Item added to cart!";
        return RedirectToAction(nameof(Index));
    }

    // POST: /Cart/Update
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(int cartItemId, int quantity)
    {
        var result = await _cartService.UpdateQuantityAsync(CurrentUserId, cartItemId, quantity);

        if (!result.Success)
            TempData["Error"] = result.Error;

        return RedirectToAction(nameof(Index));
    }

    // POST: /Cart/Remove
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Remove(int cartItemId)
    {
        await _cartService.RemoveItemAsync(CurrentUserId, cartItemId);
        TempData["Success"] = "Item removed from cart.";
        return RedirectToAction(nameof(Index));
    }
}