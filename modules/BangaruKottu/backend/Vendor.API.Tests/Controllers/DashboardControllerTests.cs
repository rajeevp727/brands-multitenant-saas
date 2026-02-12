using Moq;
using Xunit;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Vendor.API.Controllers;
using Vendor.Application.DTOs.Dashboard;
using Vendor.Application.Interfaces;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Vendor.API.Tests.Controllers
{
    public class DashboardControllerTests
    {
        private readonly Mock<IDashboardService> _mockDashboardService;
        private readonly Mock<ILogger<DashboardController>> _mockLogger;
        private readonly DashboardController _controller;

        public DashboardControllerTests()
        {
            _mockDashboardService = new Mock<IDashboardService>();
            _mockLogger = new Mock<ILogger<DashboardController>>();
            _controller = new DashboardController(_mockDashboardService.Object, _mockLogger.Object);
            
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim("VendorId", "123")
            }, "mock"));

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
        }

        [Fact]
        public async Task GetDashboard_ReturnsOkWithData()
        {
            // Arrange
            var data = new DashboardDto { RevenueToday = 1000 };
            _mockDashboardService.Setup(s => s.GetDashboardDataAsync(123)).ReturnsAsync(data);

            // Act
            var result = await _controller.GetDashboard();

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            okResult.Value.Should().BeEquivalentTo(data);
        }
    }
}
