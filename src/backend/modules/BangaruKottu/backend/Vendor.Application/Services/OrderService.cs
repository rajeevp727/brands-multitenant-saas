using Mapster;
using Vendor.Application.DTOs.Order;
using Vendor.Application.Interfaces;

namespace Vendor.Application.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;

    public OrderService(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<IEnumerable<OrderDto>> GetVendorOrdersAsync(int vendorId)
    {
        var orders = await _orderRepository.GetByVendorIdAsync(vendorId);
        return orders.Adapt<IEnumerable<OrderDto>>();
    }

    public async Task<OrderDto?> GetOrderByIdAsync(int orderId, int vendorId)
    {
        var order = await _orderRepository.GetOrderWithItemsAsync(orderId);
        if (order == null || order.VendorId != vendorId)
            return null;

        return order.Adapt<OrderDto>();
    }

    public async Task<bool> AcceptOrderAsync(int orderId, int vendorId)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order == null || order.VendorId != vendorId || order.OrderStatus != "Pending")
            return false;

        order.OrderStatus = "Accepted";
        order.AcceptedAt = DateTime.UtcNow;
        await _orderRepository.UpdateAsync(order);
        return true;
    }

    public async Task<bool> RejectOrderAsync(int orderId, int vendorId)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order == null || order.VendorId != vendorId || order.OrderStatus != "Pending")
            return false;

        order.OrderStatus = "Rejected";
        await _orderRepository.UpdateAsync(order);
        return true;
    }

    public async Task<bool> MarkPreparingAsync(int orderId, int vendorId)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order == null || order.VendorId != vendorId || order.OrderStatus != "Accepted")
            return false;

        order.OrderStatus = "Preparing";
        order.PreparedAt = DateTime.UtcNow;
        await _orderRepository.UpdateAsync(order);
        return true;
    }

    public async Task<bool> DispatchOrderAsync(int orderId, int vendorId)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order == null || order.VendorId != vendorId || order.OrderStatus != "Preparing")
            return false;

        order.OrderStatus = "Dispatched";
        order.DispatchedAt = DateTime.UtcNow;
        await _orderRepository.UpdateAsync(order);
        return true;
    }

    public async Task<bool> MarkDeliveredAsync(int orderId, int vendorId)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order == null || order.VendorId != vendorId || order.OrderStatus != "Dispatched")
            return false;

        order.OrderStatus = "Delivered";
        order.DeliveredAt = DateTime.UtcNow;
        await _orderRepository.UpdateAsync(order);
        return true;
    }
}

