using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VKart.ServicesLayer.Implementations;
using VKart.ServicesLayer.Interfaces;

namespace VKart.ServicesLayer;

public static class DependencyInjection
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ICartService, CartService>();
        services.AddScoped<IOrderService, OrderService>();
        return services;
    }
}