using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using VKart.ModelsLayer.DTOs;
using VKart.ModelsLayer.Entities;
using VKart.ServicesLayer.Common;
using VKart.ServicesLayer.Interfaces;

namespace VKart.ServicesLayer.Implementations;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly IConfiguration _config;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        IConfiguration config)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _config = config;
    }

    public async Task<ServiceResult<AuthResponseDto>> RegisterAsync(RegisterDto dto)
    {
        // Duplicate email check
        var existing = await _userManager.FindByEmailAsync(dto.Email);
        if (existing is not null)
            return ServiceResult<AuthResponseDto>.Fail("Email already registered.");

        var user = new ApplicationUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            FullName = dto.FullName,
            MobileNumber = dto.MobileNumber,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
            return ServiceResult<AuthResponseDto>.Fail(
                string.Join("; ", result.Errors.Select(e => e.Description)));

        // Always Customer role on public registration (never let user pick Admin)
        if (!await _roleManager.RoleExistsAsync("Customer"))
            await _roleManager.CreateAsync(new ApplicationRole { Name = "Customer" });

        await _userManager.AddToRoleAsync(user, "Customer");

        return ServiceResult<AuthResponseDto>.Ok(await BuildAuthResponseAsync(user));
    }

    public async Task<ServiceResult<AuthResponseDto>> LoginAsync(LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user is null)
            return ServiceResult<AuthResponseDto>.Fail("Invalid email or password.");

        if (!await _userManager.CheckPasswordAsync(user, dto.Password))
            return ServiceResult<AuthResponseDto>.Fail("Invalid email or password.");

        return ServiceResult<AuthResponseDto>.Ok(await BuildAuthResponseAsync(user));
    }

    public async Task<ServiceResult<AuthResponseDto>> GetCurrentUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null) return ServiceResult<AuthResponseDto>.Fail("User not found.");
        return ServiceResult<AuthResponseDto>.Ok(await BuildAuthResponseAsync(user));
    }

    private async Task<AuthResponseDto> BuildAuthResponseAsync(ApplicationUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var expiryMinutes = int.Parse(_config["Jwt:ExpiryMinutes"] ?? "120");
        var expiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new("fullName", user.FullName)
        };
        foreach (var r in roles) claims.Add(new Claim(ClaimTypes.Role, r));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: expiresAt,
            signingCredentials: creds);

        return new AuthResponseDto
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            Email = user.Email!,
            FullName = user.FullName,
            UserId = user.Id,
            Roles = roles,
            ExpiresAt = expiresAt
        };
    }
}