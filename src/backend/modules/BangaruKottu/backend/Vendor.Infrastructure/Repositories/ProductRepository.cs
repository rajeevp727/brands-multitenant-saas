using Microsoft.EntityFrameworkCore;
using Vendor.Application.Interfaces;
using Vendor.Domain.Entities;
using Vendor.Infrastructure.Data;

namespace Vendor.Infrastructure.Repositories;

public class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Product>> GetByVendorIdAsync(int vendorId)
    {
        return await _dbSet
            .Include(p => p.Category)
            .Where(p => p.VendorId == vendorId)
            .ToListAsync();
    }

    public async Task<Product?> GetProductWithDetailsAsync(int productId)
    {
        return await _dbSet
            .Include(p => p.Category)
            .Include(p => p.Vendor)
            .FirstOrDefaultAsync(p => p.ProductId == productId);
    }
}

