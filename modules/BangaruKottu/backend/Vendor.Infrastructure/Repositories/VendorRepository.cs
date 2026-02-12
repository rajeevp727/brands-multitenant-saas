using Microsoft.EntityFrameworkCore;
using Vendor.Application.Interfaces;
using Vendor.Domain.Entities;
using Vendor.Infrastructure.Data;

namespace Vendor.Infrastructure.Repositories;

public class VendorRepository : Repository<VendorEntity>, IVendorRepository
{
    public VendorRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<VendorEntity?> GetByUserIdAsync(int userId)
    {
        return await _dbSet.FirstOrDefaultAsync(v => v.UserId == userId);
    }

    public async Task<VendorEntity?> GetVendorWithUserAsync(int vendorId)
    {
        return await _dbSet
            .Include(v => v.User)
            .FirstOrDefaultAsync(v => v.VendorId == vendorId);
    }
}

