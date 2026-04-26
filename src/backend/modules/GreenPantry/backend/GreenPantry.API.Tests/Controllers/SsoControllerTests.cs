using Moq;
using Xunit;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using GreenPantry.API.Controllers;
using GreenPantry.Application.DTOs.Auth;
using GreenPantry.Application.Interfaces;
using GreenPantry.Domain.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace GreenPantry.API.Tests.Controllers
{
    public class SsoControllerTests
    {
        private readonly Mock<IAuthService> _mockAuthService;
        private readonly Mock<IUserRepository> _mockUserRepo;
        private readonly Mock<IConfiguration> _mockConfig;
        private readonly Mock<ILogger<SsoController>> _mockLogger;
        private readonly SsoController _controller;

        public SsoControllerTests()
        {
            _mockAuthService = new Mock<IAuthService>();
            _mockUserRepo = new Mock<IUserRepository>();
            _mockConfig = new Mock<IConfiguration>();
            _mockLogger = new Mock<ILogger<SsoController>>();
            
            _controller = new SsoController(_mockAuthService.Object, _mockUserRepo.Object, _mockConfig.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task SyncSsoUser_ExistingUser_ReturnsOkWithLoginResponse()
        {
            // Arrange
            var email = "test@example.com";
            var request = new SsoSyncRequest { Email = email };
            var existingUser = new User { Id = "user_123", Email = email };
            var authResponse = new AuthResponse { Token = "token_123" };

            _mockUserRepo.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync(existingUser);
            _mockAuthService.Setup(s => s.LoginAsync(It.IsAny<LoginRequest>())).ReturnsAsync(authResponse);

            // Act
            var result = await _controller.SyncSsoUser(request);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            okResult.Value.Should().BeEquivalentTo(authResponse);
        }

        [Fact]
        public async Task SyncSsoUser_NewUser_ReturnsOkWithRegisterResponse()
        {
            // Arrange
            var email = "new@example.com";
            var request = new SsoSyncRequest { Email = email };
            var authResponse = new AuthResponse { Token = "token_new" };

            _mockUserRepo.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync((User)null);
            _mockAuthService.Setup(s => s.RegisterAsync(It.IsAny<RegisterRequest>())).ReturnsAsync(authResponse);

            // Act
            var result = await _controller.SyncSsoUser(request);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            okResult.Value.Should().BeEquivalentTo(authResponse);
        }

        [Fact]
        public async Task SyncSsoUser_Error_Returns500()
        {
            // Arrange
            var request = new SsoSyncRequest { Email = "error@example.com" };
            _mockUserRepo.Setup(r => r.GetByEmailAsync(It.IsAny<string>())).ThrowsAsync(new Exception("DB Error"));

            // Act
            var result = await _controller.SyncSsoUser(request);

            // Assert
            result.Result.Should().BeOfType<ObjectResult>();
            var objectResult = result.Result as ObjectResult;
            objectResult.StatusCode.Should().Be(500);
        }
    }
}
