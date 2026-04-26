using Moq;
using Xunit;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using GreenPantry.API.Controllers;
using GreenPantry.Application.DTOs.Auth;
using GreenPantry.Application.Interfaces;
using GreenPantry.Infrastructure.Data;
using System.Threading.Tasks;

namespace GreenPantry.API.Tests.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthService> _mockAuthService;
        private readonly Mock<IUserRepository> _mockUserRepo;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _mockAuthService = new Mock<IAuthService>();
            _mockUserRepo = new Mock<IUserRepository>();
            _controller = new AuthController(_mockAuthService.Object, _mockUserRepo.Object);
        }

        [Fact]
        public async Task Login_ValidCredentials_ReturnsOk()
        {
            // Arrange
            var response = new AuthResponse { Token = "token123" };
            _mockAuthService.Setup(s => s.LoginAsync(It.IsAny<LoginRequest>())).ReturnsAsync(response);

            // Act
            var result = await _controller.Login(new LoginRequest());

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            okResult.Value.Should().BeEquivalentTo(response);
        }

        [Fact]
        public async Task Register_ValidData_ReturnsOk()
        {
            // Arrange
            var response = new AuthResponse { Token = "token123" };
            _mockAuthService.Setup(s => s.RegisterAsync(It.IsAny<RegisterRequest>())).ReturnsAsync(response);

            // Act
            var result = await _controller.Register(new RegisterRequest());

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            okResult.Value.Should().BeEquivalentTo(response);
        }
    }
}
