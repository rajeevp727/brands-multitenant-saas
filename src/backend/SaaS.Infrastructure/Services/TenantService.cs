using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore;
using SaaS.Domain.Entities;
using SaaS.Infrastructure.Repositories;
using SaaS.Infrastructure.Persistence;

namespace SaaS.Infrastructure.Services
{
    public class TenantService : ITenantService
    {
        private readonly IMemoryCache _cache;
        private readonly ApplicationDbContext _db;

        public TenantService(IMemoryCache cache, ApplicationDbContext db)
        {
            _cache = cache;
            _db = db;
        }

        public async Task<Tenant?> GetTenantByHostAsync(string host)
        {
            return await _cache.GetOrCreateAsync($"tenant_{host}", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(20);

                return await _db.Tenants
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t => t.Hostname == host);
            });
        }
    }
}
