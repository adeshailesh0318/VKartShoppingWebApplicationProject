using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VKart.ModelsLayer.Entities;

namespace VKart.DataAccessLayer.Seed;

public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var sp = scope.ServiceProvider;

        var roleManager = sp.GetRequiredService<RoleManager<ApplicationRole>>();
        var userManager = sp.GetRequiredService<UserManager<ApplicationUser>>();
        var context = sp.GetRequiredService<Context.VKartDbContext>();
        var logger = sp.GetRequiredService<ILoggerFactory>().CreateLogger("DbSeeder");

        try
        {
            // 1. Roles
            foreach (var role in new[] { "Admin", "Customer" })
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new ApplicationRole { Name = role, Description = $"{role} role" });
                    logger.LogInformation("Created role {Role}", role);
                }
            }

            // 2. Development admin (password from env or fallback)
            const string adminEmail = "admin@vcart.com";
            var admin = await userManager.FindByEmailAsync(adminEmail);
            if (admin is null)
            {
                admin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "V-Cart Admin",
                    MobileNumber = "9999999999",
                    EmailConfirmed = true
                };
                // ⚠️ Never hardcode production passwords. For dev only.
                var devPassword = Environment.GetEnvironmentVariable("VCART_ADMIN_PASSWORD") ?? "Admin@123";
                var result = await userManager.CreateAsync(admin, devPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                    logger.LogInformation("Created dev admin {Email}", adminEmail);
                }
                else
                {
                    logger.LogError("Admin creation failed: {Errors}",
                        string.Join("; ", result.Errors.Select(e => e.Description)));
                }
            }

            // 3. Categories
            if (!context.Categories.Any())
            {
                context.Categories.AddRange(
                    new Category { CategoryName = "Electronics", Description = "Phones, headphones, wearables" },
                    new Category { CategoryName = "Fashion", Description = "Clothing, shoes, accessories" },
                    new Category { CategoryName = "Home & Kitchen", Description = "Appliances, cookware" },
                    new Category { CategoryName = "Books", Description = "Fiction, non-fiction" },
                    new Category { CategoryName = "Sports", Description = "Fitness, outdoor" }
                );
                await context.SaveChangesAsync();
            }

            // 4. Sample products
            if (!context.Products.Any())
            {
                var electronics = context.Categories.First(c => c.CategoryName == "Electronics").CategoryId;
                var fashion = context.Categories.First(c => c.CategoryName == "Fashion").CategoryId;
                var home = context.Categories.First(c => c.CategoryName == "Home & Kitchen").CategoryId;
                var books = context.Categories.First(c => c.CategoryName == "Books").CategoryId;
                var sports = context.Categories.First(c => c.CategoryName == "Sports").CategoryId;

                context.Products.AddRange(
                    new Product { ProductName = "Smartphone X10 Pro", Description = "6.5\" OLED, 128GB, 5G", Price = 24999, StockQuantity = 12, ImageUrl = "https://via.placeholder.com/300x200?text=Smartphone", CategoryId = electronics },
                    new Product { ProductName = "Noise Cancelling Headphones", Description = "Over-ear, 30h battery", Price = 3499, StockQuantity = 8, ImageUrl = "https://via.placeholder.com/300x200?text=Headphones", CategoryId = electronics },
                    new Product { ProductName = "Fitness Smartwatch", Description = "Heart rate, SpO2, AMOLED", Price = 1999, StockQuantity = 5, ImageUrl = "https://via.placeholder.com/300x200?text=Smartwatch", CategoryId = electronics },
                    new Product { ProductName = "Men's Cotton T-Shirt", Description = "Slim fit, pack of 2", Price = 799, StockQuantity = 24, ImageUrl = "https://via.placeholder.com/300x200?text=T-Shirt", CategoryId = fashion },
                    new Product { ProductName = "Travel Backpack 30L", Description = "Water-resistant, laptop sleeve", Price = 1299, StockQuantity = 9, ImageUrl = "https://via.placeholder.com/300x200?text=Backpack", CategoryId = fashion },
                    new Product { ProductName = "Coffee Maker 1.5L", Description = "Drip filter, 800W", Price = 2199, StockQuantity = 15, ImageUrl = "https://via.placeholder.com/300x200?text=Coffee+Maker", CategoryId = home },
                    new Product { ProductName = "The Silent Patient", Description = "Paperback, bestseller", Price = 399, StockQuantity = 30, ImageUrl = "https://via.placeholder.com/300x200?text=Book", CategoryId = books },
                    new Product { ProductName = "Eco Yoga Mat", Description = "Anti-slip, 6mm", Price = 899, StockQuantity = 0, ImageUrl = "https://via.placeholder.com/300x200?text=Yoga+Mat", CategoryId = sports }
                );
                await context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Seeding failed");
            throw;
        }
    }
}