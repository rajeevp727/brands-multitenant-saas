using Microsoft.Extensions.DependencyInjection;
using SaaS.Application.Common;
using System.Reflection;

namespace SaaS.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddScoped(typeof(IGenericService<,>), typeof(GenericService<,>));
        services.AddScoped<SaaS.Application.Services.IAuthService, SaaS.Application.Services.AuthService>();

        return services;
    }
}
