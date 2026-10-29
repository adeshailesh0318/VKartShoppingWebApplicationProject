using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VKart.ModelsLayer.Entities;
using VKart.PresentationLayer.ViewModels;

namespace VKart.PresentationLayer.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<ApplicationRole> _roleManager;

    public AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<ApplicationRole> roleManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
    }

    // ---- REGISTER ----
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Register() => View();

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var existing = await _userManager.FindByEmailAsync(vm.Email);
        if (existing is not null)
        {
            ModelState.AddModelError(nameof(vm.Email), "Email already registered.");
            return View(vm);
        }

        var user = new ApplicationUser
        {
            UserName = vm.Email,
            Email = vm.Email,
            FullName = vm.FullName,
            MobileNumber = vm.MobileNumber,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, vm.Password);
        if (!result.Succeeded)
        {
            foreach (var err in result.Errors)
                ModelState.AddModelError("", err.Description);
            return View(vm);
        }

        // Always Customer role — never Admin from public registration
        if (!await _roleManager.RoleExistsAsync("Customer"))
            await _roleManager.CreateAsync(new ApplicationRole { Name = "Customer" });
        await _userManager.AddToRoleAsync(user, "Customer");

        TempData["Success"] = "Registration successful! Please log in.";
        return RedirectToAction(nameof(Login));
    }

    // ---- LOGIN ----
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
        => View(new LoginViewModel { ReturnUrl = returnUrl });

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var user = await _userManager.FindByEmailAsync(vm.Email);
        if (user is null)
        {
            ModelState.AddModelError("", "Invalid email or password.");
            return View(vm);
        }

        var result = await _signInManager.PasswordSignInAsync(
            user, vm.Password, vm.RememberMe, lockoutOnFailure: false);

        if (!result.Succeeded)
        {
            ModelState.AddModelError("", "Invalid email or password.");
            return View(vm);
        }

        // Redirect to returnUrl or Home
        if (!string.IsNullOrEmpty(vm.ReturnUrl) && Url.IsLocalUrl(vm.ReturnUrl))
            return Redirect(vm.ReturnUrl);

        return RedirectToAction("Index", "Home");
    }

    // ---- LOGOUT ----
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        TempData["Success"] = "You have been logged out.";
        return RedirectToAction("Index", "Home");
    }

    // ---- ACCESS DENIED ----
    [HttpGet]
    public IActionResult AccessDenied() => View();
}