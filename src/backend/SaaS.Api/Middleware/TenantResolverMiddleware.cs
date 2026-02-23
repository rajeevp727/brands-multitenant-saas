using SaaS.Api.Core.Tenancy;

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

            tenantContext.TenantId = tenant.Id;
            tenantContext.Name = tenant.Name;
            tenantContext.Domain = tenant.Domain;
            tenantContext.ConnectionString = tenant.ConnectionString;

            context.Items["Tenant"] = tenantContext;

            await _next(context);
        }
    }
}
