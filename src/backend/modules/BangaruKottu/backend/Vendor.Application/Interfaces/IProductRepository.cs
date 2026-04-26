using Vendor.Domain.Entities;

namespace Vendor.Application.Interfaces;

public interface IProductRepository : IRepository<Product>
{
    Task<IEnumerable<Product>> GetByVendorIdAsync(int vendorId);
    Task<Product?> GetProductWithDetailsAsync(int productId);
}

