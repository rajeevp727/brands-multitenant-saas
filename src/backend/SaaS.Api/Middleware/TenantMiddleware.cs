using Microsoft.AspNetCore.Http;
using SaaS.Domain.Interfaces;

namespace SaaS.Api.Middleware;

public class TenantMiddleware
{
    private readonly RequestDelegate _next;

    public TenantMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ITenantProvider tenantProvider)
    {
        var tenantId = tenantProvider.GetTenantId();

        if (string.IsNullOrEmpty(tenantId))
        {
            // context.Response.StatusCode = StatusCodes.Status400BadRequest;
            // await context.Response.WriteAsync("Tenant ID is required.");
            // return;
            // For now, let it pass or set a default for admin
        }

        // Add tenant info to context for logging / audit
        context.Items["TenantId"] = tenantId;
        
        using (Serilog.Context.LogContext.PushProperty("TenantId", tenantId ?? "Global"))
        {
            await _next(context);
        }
    }
}
