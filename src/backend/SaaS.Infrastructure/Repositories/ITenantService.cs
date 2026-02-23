using SaaS.Domain.Entities;

namespace SaaS.Infrastructure.Repositories
{
    public interface ITenantService
    {
        Task<Tenant?> GetTenantByHostAsync(string host);
    }
}
