using Vendor.Application.DTOs.Dashboard;
using Vendor.Application.Interfaces;

namespace Vendor.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly IOrderRepository _orderRepository;

    public DashboardService(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<DashboardDto> GetDashboardDataAsync(int vendorId)
    {
        var ordersToday = await _orderRepository.GetTodayOrdersCountAsync(vendorId);
        var revenueToday = await _orderRepository.GetTodayRevenueAsync(vendorId);
        var pendingOrders = await _orderRepository.GetPendingOrdersCountAsync(vendorId);
        var deliveredOrders = await _orderRepository.GetDeliveredOrdersCountAsync(vendorId);

        return new DashboardDto
        {
            OrdersToday = ordersToday,
            RevenueToday = revenueToday,
            PendingOrders = pendingOrders,
            DeliveredOrders = deliveredOrders
        };
    }
}

