using Microsoft.Extensions.DependencyInjection;
using Mapster;
using SaaS.Application.Mappings;
using SaaS.Application.Common;
using System.Reflection;

namespace SaaS.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        MapsterConfig.Configure();
        services.AddMapster();
        services.AddScoped(typeof(IGenericService<,>), typeof(GenericService<,>));
        services.AddScoped<SaaS.Application.Services.IAuthService, SaaS.Application.Services.AuthService>();

        return services;
    }
}
