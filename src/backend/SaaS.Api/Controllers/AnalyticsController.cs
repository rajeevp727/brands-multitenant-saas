using Microsoft.AspNetCore.Mvc;
using SaaS.Application.DTOs;
using Microsoft.EntityFrameworkCore;
using SaaS.Infrastructure.Persistence;

namespace SaaS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnalyticsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public AnalyticsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("command-center")]
    public async Task<IActionResult> GetCommandCenterStats()
    {
        var todayUtc = DateTime.UtcNow.Date;

        // All brands from DB (no tenant filter for corporate dashboard)
        var brands = await _context.Brands
            .IgnoreQueryFilters()
            .Select(b => new { b.Id, b.TenantId, b.Name })
            .ToListAsync();

        var brandMetrics = new List<BrandMetricDto>();

        foreach (var b in brands)
        {
            var ordersToday = await _context.Orders
                .IgnoreQueryFilters()
                .Where(o => o.TenantId == b.TenantId && o.CreatedAt >= todayUtc && o.CreatedAt < todayUtc.AddDays(1))
                .ToListAsync();

            brandMetrics.Add(new BrandMetricDto
            {
                BrandId = b.TenantId,
                BrandName = b.Name,
                TodayRevenue = ordersToday.Sum(o => o.TotalAmount),
                TodayOrders = ordersToday.Count,
                Status = "Live"
            });
        }

        var totalRevenue = brandMetrics.Sum(m => m.TodayRevenue);
        var totalOrdersToday = brandMetrics.Sum(m => m.TodayOrders);
        var activeVendors = await _context.Users
            .IgnoreQueryFilters()
            .Where(u => u.RoleId != null && _context.Roles.IgnoreQueryFilters().Any(r => r.Id == u.RoleId && r.Name == "Vendor"))
            .CountAsync();

        var stats = new CommandCenterStatsDto
        {
            TotalRevenue = totalRevenue,
            OrdersToday = totalOrdersToday,
            ActiveVendors = activeVendors,
            SystemLoad = 24.5,
            BrandMetrics = brandMetrics
        };

        return Ok(stats);
    }
}
