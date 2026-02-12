using Vendor.Domain.Entities;

namespace Vendor.Application.Interfaces;

public interface IVendorRepository : IRepository<VendorEntity>
{
    Task<VendorEntity?> GetByUserIdAsync(int userId);
    Task<VendorEntity?> GetVendorWithUserAsync(int vendorId);
}

