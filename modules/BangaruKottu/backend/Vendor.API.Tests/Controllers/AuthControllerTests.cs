using Moq;
using Xunit;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Vendor.API.Controllers;
using Vendor.Application.DTOs.Auth;
using Vendor.Application.Interfaces;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Vendor.API.Tests.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthService> _mockAuthService;
        private readonly Mock<ILogger<AuthController>> _mockLogger;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _mockAuthService = new Mock<IAuthService>();
            _mockLogger = new Mock<ILogger<AuthController>>();
            _controller = new AuthController(_mockAuthService.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task Login_ValidCredentials_ReturnsOk()
        {
            // Arrange
            var response = new LoginResponse { Token = "token123" };
            _mockAuthService.Setup(s => s.LoginAsync(It.IsAny<LoginRequest>())).ReturnsAsync(response);

            // Act
            var result = await _controller.Login(new LoginRequest());

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            okResult.Value.Should().BeEquivalentTo(response);
        }

        [Fact]
        public async Task Login_InvalidCredentials_ReturnsUnauthorized()
        {
            // Arrange
            _mockAuthService.Setup(s => s.LoginAsync(It.IsAny<LoginRequest>())).ReturnsAsync((LoginResponse)null);

            // Act
            var result = await _controller.Login(new LoginRequest());

            // Assert
            result.Result.Should().BeOfType<UnauthorizedObjectResult>();
        }
    }
}
