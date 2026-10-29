using Microsoft.Extensions.DependencyInjection;
using VKart.RepositoriesLayer.Implementations;
using VKart.RepositoriesLayer.Interfaces;

namespace VKart.RepositoriesLayer;

public static class DependencyInjection
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICartRepository, CartRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        return services;
    }
}