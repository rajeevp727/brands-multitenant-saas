using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SaaS.Domain.Entities;
using SaaS.Infrastructure.Persistence;
using SaaS.Application.Interfaces;

namespace SaaS.Infrastructure.Services;

public class SystemService : ISystemService
{
    private readonly ApplicationDbContext _context;
    private const string DeploymentStatusKey = "SYSTEM_DEPLOYMENT_ACTIVE";

    public SystemService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> IsDeploymentActiveAsync()
    {
        // Use IgnoreQueryFilters because AppConstants might have tenant filters, 
        // but this is a global system flag.
        var constant = await _context.AppConstants
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(c => c.ConstantKey == DeploymentStatusKey);
            
        return constant?.ConstantValue?.ToLower() == "true";
    }

    public async Task SetDeploymentStatusAsync(bool isActive)
    {
        var constant = await _context.AppConstants
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(c => c.ConstantKey == DeploymentStatusKey);

        if (constant == null)
        {
            constant = new AppConstant
            {
                Id = Guid.NewGuid(),
                ConstantKey = DeploymentStatusKey,
                ConstantValue = isActive.ToString().ToLower(),
                Category = "System",
                TenantId = null // Global
            };
            await _context.AppConstants.AddAsync(constant);
        }
        else
        {
            constant.ConstantValue = isActive.ToString().ToLower();
        }

        await _context.SaveChangesAsync();
    }
}
