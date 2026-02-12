using System.Collections.Generic;

namespace SaaS.Application.DTOs;

public class CommandCenterStatsDto
{
    public decimal TotalRevenue { get; set; }
    public int OrdersToday { get; set; }
    public int ActiveVendors { get; set; }
    public double SystemLoad { get; set; }
    public List<BrandMetricDto> BrandMetrics { get; set; } = new();
}

public class BrandMetricDto
{
    public string BrandId { get; set; } = string.Empty;
    public string BrandName { get; set; } = string.Empty;
    public decimal TodayRevenue { get; set; }
    public int TodayOrders { get; set; }
    public string Status { get; set; } = "Live";
}
