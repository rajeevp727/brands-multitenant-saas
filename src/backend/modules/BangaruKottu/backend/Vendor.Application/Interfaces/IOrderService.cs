using Vendor.Application.DTOs.Order;

namespace Vendor.Application.Interfaces;

public interface IOrderService
{
    Task<IEnumerable<OrderDto>> GetVendorOrdersAsync(int vendorId);
    Task<OrderDto?> GetOrderByIdAsync(int orderId, int vendorId);
    Task<bool> AcceptOrderAsync(int orderId, int vendorId);
    Task<bool> RejectOrderAsync(int orderId, int vendorId);
    Task<bool> MarkPreparingAsync(int orderId, int vendorId);
    Task<bool> DispatchOrderAsync(int orderId, int vendorId);
    Task<bool> MarkDeliveredAsync(int orderId, int vendorId);
}

