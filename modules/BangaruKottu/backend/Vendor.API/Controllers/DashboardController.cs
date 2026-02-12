using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vendor.API.Extensions;
using Vendor.Application.DTOs.Dashboard;
using Vendor.Application.Interfaces;

namespace Vendor.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Vendor")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(IDashboardService dashboardService, ILogger<DashboardController> logger)
    {
        _dashboardService = dashboardService;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(DashboardDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<DashboardDto>> GetDashboard()
    {
        var vendorId = User.GetVendorId();
        var dashboard = await _dashboardService.GetDashboardDataAsync(vendorId);
        return Ok(dashboard);
    }
}

