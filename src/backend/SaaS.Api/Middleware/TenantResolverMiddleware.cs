using SaaS.Api.Core.Tenancy;
using SaaS.Infrastructure.Repositories;

namespace SaaS.Api.Middleware
{
    public class TenantResolutionMiddleware
    {
        private readonly RequestDelegate _next;

        public TenantResolutionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, TenantContext tenantContext, ITenantService tenantService)
        {
            var host = context.Request.Host.Host.ToLower();

            // example: greenpantry.localhost or greenpantry.assetforge.in
            var tenant = await tenantService.GetTenantByHostAsync(host);

            if (tenant == null)
            {
                context.Response.StatusCode = 404;
                await context.Response.WriteAsync("Tenant not configured");
                return;
            }

            tenantContext.TenantId = Guid.Parse(tenant.Id);
            tenantContext.Name = tenant.Name;
            tenantContext.Domain = tenant.Hostname;
            tenantContext.ConnectionString = ""; // TODO: Add connection string logic if needed

            context.Items["Tenant"] = tenantContext;

            await _next(context);
        }
    }
}
