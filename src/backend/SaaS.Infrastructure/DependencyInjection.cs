using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SaaS.Domain.Interfaces;
using SaaS.Infrastructure.Persistence;
using SaaS.Infrastructure.Repositories;

namespace SaaS.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            // options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
            //     b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IJwtProvider, SaaS.Infrastructure.Auth.JwtProvider>();
        services.AddScoped<SaaS.Application.Interfaces.INotificationService, SaaS.Infrastructure.Services.NotificationService>();

        return services;
    }
}
