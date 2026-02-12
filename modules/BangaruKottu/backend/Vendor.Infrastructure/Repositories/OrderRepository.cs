using Microsoft.EntityFrameworkCore;
using Vendor.Application.Interfaces;
using Vendor.Domain.Entities;
using Vendor.Infrastructure.Data;

namespace Vendor.Infrastructure.Repositories;

public class OrderRepository : Repository<Order>, IOrderRepository
{
    public OrderRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Order>> GetByVendorIdAsync(int vendorId)
    {
        return await _dbSet
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .Where(o => o.VendorId == vendorId)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }

    public async Task<Order?> GetOrderWithItemsAsync(int orderId)
    {
        return await _dbSet
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(o => o.OrderId == orderId);
    }

    public async Task<IEnumerable<Order>> GetOrdersByStatusAsync(int vendorId, string status)
    {
        return await _dbSet
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .Where(o => o.VendorId == vendorId && o.OrderStatus == status)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }

    public async Task<int> GetTodayOrdersCountAsync(int vendorId)
    {
        var today = DateTime.UtcNow.Date;
        return await _dbSet
            .CountAsync(o => o.VendorId == vendorId && o.OrderDate.Date == today);
    }

    public async Task<decimal> GetTodayRevenueAsync(int vendorId)
    {
        var today = DateTime.UtcNow.Date;
        return await _dbSet
            .Where(o => o.VendorId == vendorId && o.OrderDate.Date == today && o.OrderStatus == "Delivered")
            .SumAsync(o => o.TotalAmount);
    }

    public async Task<int> GetPendingOrdersCountAsync(int vendorId)
    {
        return await _dbSet
            .CountAsync(o => o.VendorId == vendorId && (o.OrderStatus == "Pending" || o.OrderStatus == "Accepted" || o.OrderStatus == "Preparing"));
    }

    public async Task<int> GetDeliveredOrdersCountAsync(int vendorId)
    {
        return await _dbSet
            .CountAsync(o => o.VendorId == vendorId && o.OrderStatus == "Delivered");
    }
}

