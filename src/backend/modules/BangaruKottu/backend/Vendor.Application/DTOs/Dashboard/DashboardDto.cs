namespace Vendor.Application.DTOs.Dashboard;

public class DashboardDto
{
    public int OrdersToday { get; set; }
    public decimal RevenueToday { get; set; }
    public int PendingOrders { get; set; }
    public int DeliveredOrders { get; set; }
}

