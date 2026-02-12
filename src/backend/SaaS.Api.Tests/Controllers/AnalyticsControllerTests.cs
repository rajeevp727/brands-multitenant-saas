using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using SaaS.Api.Controllers;
using SaaS.Application.DTOs;
using SaaS.Infrastructure.Persistence;
using SaaS.Domain.Interfaces;
using Xunit;

namespace SaaS.Api.Tests.Controllers;

public class AnalyticsControllerTests
{
    private readonly ApplicationDbContext _context;
    private readonly AnalyticsController _controller;

    public AnalyticsControllerTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
            
        var mockTenantProvider = new Mock<ITenantProvider>();
        _context = new ApplicationDbContext(options, mockTenantProvider.Object);
        _controller = new AnalyticsController(_context);
    }

    [Fact]
    public async Task GetCommandCenterStats_ReturnsOk_WithExpectedMetrics()
    {
        // Act
        var result = await _controller.GetCommandCenterStats();

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        var stats = okResult.Value as CommandCenterStatsDto;
        
        stats.Should().NotBeNull();
        stats.TotalRevenue.Should().Be(842900m);
        stats.BrandMetrics.Should().HaveCount(6);
        stats.BrandMetrics[0].BrandName.Should().Be("GreenPantry");
    }
}
