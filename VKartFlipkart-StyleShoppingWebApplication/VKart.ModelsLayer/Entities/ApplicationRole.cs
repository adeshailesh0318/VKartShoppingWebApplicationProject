using Microsoft.AspNetCore.Identity;

namespace VKart.ModelsLayer.Entities;

public class ApplicationRole : IdentityRole
{
    public string? Description { get; set; }
}