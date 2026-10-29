using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VKart.ModelsLayer.DTOs;
using VKart.ServicesLayer.Interfaces;
using VKart.WebApiLayer.Extensions;

namespace VKart.WebApiLayer.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;

    public AuthController(IAuthService auth) => _auth = auth;

    /// <summary>Register a new customer. Customer role auto-assigned.</summary>
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        var result = await _auth.RegisterAsync(dto);
        if (!result.Success) return BadRequest(new { error = result.Error });
        return Ok(result.Data);
    }

    /// <summary>Login and receive a JWT token.</summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        var result = await _auth.LoginAsync(dto);
        if (!result.Success) return Unauthorized(new { error = result.Error });
        return Ok(result.Data);
    }

    /// <summary>Get current user info from JWT.</summary>
    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> Me()
    {
        var userId = User.GetUserId();
        var result = await _auth.GetCurrentUserAsync(userId);
        if (!result.Success) return NotFound(new { error = result.Error });
        return Ok(result.Data);
    }

    /// <summary>Admin-only test endpoint — confirms role is on token.</summary>
    [HttpGet("admin-check")]
    [Authorize(Roles = "Admin")]
    public IActionResult AdminCheck()
        => Ok(new { message = "You are Admin. Access granted." });
}