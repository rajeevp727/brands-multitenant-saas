using Moq;
using Xunit;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using GreenPantry.API.Controllers;
using GreenPantry.Application.DTOs.Geolocation;
using GreenPantry.Application.Interfaces;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace GreenPantry.API.Tests.Controllers
{
    public class GeolocationControllerTests
    {
        private readonly Mock<IGeolocationService> _mockGeolocationService;
        private readonly GeolocationController _controller;

        public GeolocationControllerTests()
        {
            _mockGeolocationService = new Mock<IGeolocationService>();
            _controller = new GeolocationController(_mockGeolocationService.Object);
            
            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
            };
        }

        [Fact]
        public async Task GetLocationFromCoordinates_ReturnsOk()
        {
            // Arrange
            var response = new GeolocationResponse { FormattedAddress = "123 Main St" };
            _mockGeolocationService.Setup(s => s.GetLocationFromCoordinatesAsync(19.0, 72.0)).ReturnsAsync(response);

            // Act
            var result = await _controller.GetLocationFromCoordinates(19.0, 72.0);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            okResult.Value.Should().BeEquivalentTo(response);
        }

        [Fact]
        public async Task GetLocationFromIP_ReturnsOk()
        {
            // Arrange
            var response = new GeolocationResponse { City = "Local" };
            _mockGeolocationService.Setup(s => s.GetLocationFromIPAsync(It.IsAny<string>())).ReturnsAsync(response);
            _controller.HttpContext.Connection.RemoteIpAddress = IPAddress.Parse("127.0.0.1");

            // Act
            var result = await _controller.GetLocationFromIP();

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            okResult.Value.Should().BeEquivalentTo(response);
        }
    }
}
