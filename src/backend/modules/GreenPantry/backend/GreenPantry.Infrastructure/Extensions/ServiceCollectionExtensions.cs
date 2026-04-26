using GreenPantry.Application.Interfaces;
using GreenPantry.Infrastructure.Data;
using GreenPantry.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GreenPantry.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Add PostgreSQL DbContext
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

        // Add repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRestaurantRepository, RestaurantRepository>();
        services.AddScoped<IMenuItemRepository, MenuItemRepository>();
        services.AddScoped<IMenuItemRepository, MenuItemRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();

        return services;
    }
}
