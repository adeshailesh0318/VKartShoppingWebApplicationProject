using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace VKart.WebApiLayer.Extensions;

public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Returns the user id from the "sub" claim (JWT standard).
    /// Identity's UserManager stores the id here.
    /// </summary>
    public static string GetUserId(this ClaimsPrincipal user)
    {
        return user.FindFirstValue(JwtRegisteredClaimNames.Sub)
            ?? user.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException("User id claim missing.");
    }

    public static bool IsAdmin(this ClaimsPrincipal user)
        => user.IsInRole("Admin");
}