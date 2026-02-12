using Vendor.Application.DTOs.Dashboard;

namespace Vendor.Application.Interfaces;

public interface IDashboardService
{
    Task<DashboardDto> GetDashboardDataAsync(int vendorId);
}

