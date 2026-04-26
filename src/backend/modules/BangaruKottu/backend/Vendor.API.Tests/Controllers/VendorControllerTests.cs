using Moq;
using Xunit;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Vendor.API.Controllers;
using Vendor.Application.DTOs.Vendor;
using Vendor.Application.Interfaces;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Vendor.API.Tests.Controllers
{
    public class VendorControllerTests
    {
        private readonly Mock<IVendorService> _mockVendorService;
        private readonly Mock<ILogger<VendorController>> _mockLogger;
        private readonly VendorController _controller;

        public VendorControllerTests()
        {
            _mockVendorService = new Mock<IVendorService>();
            _mockLogger = new Mock<ILogger<VendorController>>();
            _controller = new VendorController(_mockVendorService.Object, _mockLogger.Object);
            
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
        public async Task GetProfile_ReturnsOk()
        {
            // Arrange
            var profile = new VendorProfileDto { VendorName = "V1" };
            _mockVendorService.Setup(s => s.GetVendorProfileAsync(123)).ReturnsAsync(profile);

            // Act
            var result = await _controller.GetProfile();

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            okResult.Value.Should().BeEquivalentTo(profile);
        }

        [Fact]
        public async Task UpdateProfile_ReturnsOk()
        {
            // Arrange
            var profile = new VendorProfileDto { VendorName = "V2" };
            _mockVendorService.Setup(s => s.UpdateVendorProfileAsync(123, It.IsAny<UpdateVendorRequest>())).ReturnsAsync(profile);

            // Act
            var result = await _controller.UpdateProfile(new UpdateVendorRequest());

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
        }
    }
}
