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
        if (context == null)
            return null;

        // ✅ 1. JWT Claim (MOST TRUSTED)
        var claimTenantId = context.User.FindFirst("tenantId")?.Value;
        if (!string.IsNullOrEmpty(claimTenantId))
            return claimTenantId;

        // ✅ 2. Header
        var headerTenantId = context.Request.Headers["X-Tenant-Id"].FirstOrDefault();
        if (!string.IsNullOrEmpty(headerTenantId))
            return headerTenantId;

        // ✅ 3. Dev fallback
        var host = context.Request.Host.Host;
        var port = context.Request.Host.Port;

        if (host == "localhost" || host == "127.0.0.1")
        {
            if (port == 5114) return "rajeev-pvt";
            if (port == 7001) return "green-pantry";
            if (port == 7002) return "bangaru-kottu";
        }

        // ✅ 4. Subdomain
        var parts = host.Split('.');
        if (parts.Length > 2)
            return parts[0];

        return null;
    }
}