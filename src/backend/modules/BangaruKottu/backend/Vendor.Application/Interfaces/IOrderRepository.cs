using Vendor.Domain.Entities;

namespace Vendor.Application.Interfaces;

public interface IOrderRepository : IRepository<Order>
{
    Task<IEnumerable<Order>> GetByVendorIdAsync(int vendorId);
    Task<Order?> GetOrderWithItemsAsync(int orderId);
    Task<IEnumerable<Order>> GetOrdersByStatusAsync(int vendorId, string status);
    Task<int> GetTodayOrdersCountAsync(int vendorId);
    Task<decimal> GetTodayRevenueAsync(int vendorId);
    Task<int> GetPendingOrdersCountAsync(int vendorId);
    Task<int> GetDeliveredOrdersCountAsync(int vendorId);
}

