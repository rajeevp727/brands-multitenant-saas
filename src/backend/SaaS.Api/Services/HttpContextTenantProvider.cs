using Microsoft.AspNetCore.Http;
using SaaS.Domain.Interfaces;

namespace SaaS.Api.Services;

public class HttpContextTenantProvider : ITenantProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpContextTenantProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? GetTenantId()
    {
        var context = _httpContextAccessor.HttpContext;
        if (context == null) return null;

        // 1. Priority: Validated TenantId from JWT claims (Trusted source)
        var claimTenantId = context.User.FindFirst("tenantId")?.Value;
        if (!string.IsNullOrEmpty(claimTenantId))
        {
            return claimTenantId;
        }

        // 2. Fallback: Header (Useful for unauthenticated requests like registration/login discovery)
        var headerTenantId = context.Request.Headers["X-Tenant-Id"].FirstOrDefault();
        if (!string.IsNullOrEmpty(headerTenantId))
        {
            return headerTenantId;
        }
        
        // 3. Last Resort: Hostname
        var host = context.Request.Host.Host;
        var port = context.Request.Host.Port;

        // Development / Direct API Access handling
        if (host == "localhost" || host == "127.0.0.1")
        {
            if (port == 5114) return "rajeev-pvt"; // SaaS API
            if (port == 7001) return "green-pantry"; // GreenPantry API
            if (port == 7002) return "bangaru-kottu"; // Vendor API
        }

        return context.Request.Host.Value;
    }
}
