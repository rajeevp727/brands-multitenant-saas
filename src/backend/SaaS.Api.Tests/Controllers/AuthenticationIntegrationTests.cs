using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SaaS.Api.Controllers;
using SaaS.Application.DTOs;
using SaaS.Application.Services;
using SaaS.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Xunit;

namespace SaaS.Api.Tests.Controllers;

/// <summary>
/// Comprehensive authentication integration tests for login, registration, and OAuth flows
/// </summary>
public class AuthenticationIntegrationTests
{
    private readonly Mock<IAuthService> _mockAuthService;
    private readonly Mock<IWebHostEnvironment> _mockEnv;
    private readonly AuthController _controller;

    public AuthenticationIntegrationTests()
    {
        _mockAuthService = new Mock<IAuthService>();
        _mockEnv = new Mock<IWebHostEnvironment>();
        _mockEnv.Setup(e => e.IsDevelopment()).Returns(true);
        
        _controller = new AuthController(_mockAuthService.Object, _mockEnv.Object);
        
        var httpContext = new DefaultHttpContext();
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };
    }

    [Fact]
    public async Task Login_BootstrapAdmin_WithCorrectCredentials_ShouldReturnToken()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "admin@rajeev.com",
            Password = "Pass123"
        };

        var expectedResponse = new AuthResponse
        {
            Token = "eyJhbGc.eyJzdWI.SflKxw",
            RefreshToken = "refresh-token-value",
            RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7),
            User = new UserDto
            {
                Id = Guid.NewGuid().ToString(),
                Username = "Admin",
                Email = "admin@rajeev.com",
                Role = "Admin",
                TenantId = "rajeev-pvt"
            }
        };

        _mockAuthService.Setup(s => s.LoginAsync(request))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.Login(request);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        
        var value = okResult.Value;
        value.Should().NotBeNull();
        
        // Verify the response structure
        var responseType = value.GetType();
        responseType.GetProperty("token").Should().NotBeNull();
        responseType.GetProperty("user").Should().NotBeNull();
        responseType.GetProperty("refreshToken").Should().NotBeNull();
    }

    [Fact]
    public async Task Login_WithInvalidPassword_ShouldReturnUnauthorized()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "admin@rajeev.com",
            Password = "WrongPassword123"
        };

        _mockAuthService.Setup(s => s.LoginAsync(request))
            .ThrowsAsync(new Exception("Invalid credentials"));

        // Act
        var result = await _controller.Login(request);

        // Assert
        result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    [Fact]
    public async Task Login_WithNonExistentUser_ShouldReturnUnauthorized()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "nonexistent@example.com",
            Password = "Pass123"
        };

        _mockAuthService.Setup(s => s.LoginAsync(request))
            .ThrowsAsync(new Exception("Invalid credentials"));

        // Act
        var result = await _controller.Login(request);

        // Assert
        result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    [Fact]
    public async Task Login_WithEmptyEmail_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = string.Empty,
            Password = "Pass123"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await _controller.Login(request);
        });
    }

    [Fact]
    public async Task Register_NewUser_WithValidData_ShouldReturnOkWithToken()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = "newuser@example.com",
            Password = "SecurePass123",
            Username = "newuser",
            RoleName = "Customer"
        };

        var expectedResponse = new AuthResponse
        {
            Token = "eyJhbGc.eyJzdWI.SflKxw",
            RefreshToken = "refresh-token-value",
            RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7),
            User = new UserDto
            {
                Id = Guid.NewGuid().ToString(),
                Username = request.Username,
                Email = request.Email,
                Role = "Customer",
                TenantId = "rajeev-pvt"
            }
        };

        _mockAuthService.Setup(s => s.RegisterAsync(request))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.Register(request);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task Register_DuplicateEmail_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = "existing@example.com",
            Password = "SecurePass123",
            Username = "newuser",
            RoleName = "Customer"
        };

        _mockAuthService.Setup(s => s.RegisterAsync(request))
            .ThrowsAsync(new Exception("This email is already registered with this brand. Choose another email or brand."));

        // Act
        var result = await _controller.Register(request);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task RefreshToken_ValidRefreshToken_ShouldReturnNewToken()
    {
        // Arrange
        var request = new RefreshTokenRequest
        {
            RefreshToken = "valid-refresh-token"
        };

        var expectedResponse = new AuthResponse
        {
            Token = "eyJhbGc.new_token.SflKxw",
            RefreshToken = "new-refresh-token",
            RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7),
            User = new UserDto
            {
                Id = Guid.NewGuid().ToString(),
                Username = "testuser",
                Email = "test@example.com",
                Role = "Customer",
                TenantId = "rajeev-pvt"
            }
        };

        _mockAuthService.Setup(s => s.RefreshTokenAsync(request))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.RefreshToken(request);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task RefreshToken_ExpiredToken_ShouldReturnUnauthorized()
    {
        // Arrange
        var request = new RefreshTokenRequest
        {
            RefreshToken = "expired-refresh-token"
        };

        _mockAuthService.Setup(s => s.RefreshTokenAsync(request))
            .ThrowsAsync(new Exception("Invalid or expired refresh token"));

        // Act
        var result = await _controller.RefreshToken(request);

        // Assert
        result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    [Fact]
    public async Task Logout_ShouldRemoveCookieAndReturnOk()
    {
        // Act
        var result = _controller.Logout();

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        
        var okResult = result as OkObjectResult;
        var value = okResult.Value;
        var messageProp = value?.GetType().GetProperty("message");
        messageProp?.GetValue(value).Should().Be("Logged out successfully");
    }

    [Fact]
    public void ExternalLogin_GoogleProvider_ShouldReturnChallengeResult()
    {
        // Arrange
        var mockUrlHelper = new Mock<IChallengeResultWrapper>();

        // Act
        var result = _controller.ExternalLogin("google");

        // Assert
        result.Should().BeOfType<ChallengeResult>();
        (result as ChallengeResult).AuthenticationSchemes.Should().Contain("Google");
    }

    [Fact]
    public void ExternalLogin_FacebookProvider_ShouldReturnChallengeResult()
    {
        // Arrange
        // Act
        var result = _controller.ExternalLogin("facebook");

        // Assert
        result.Should().BeOfType<ChallengeResult>();
        (result as ChallengeResult).AuthenticationSchemes.Should().Contain("Facebook");
    }

    [Fact]
    public void ExternalLogin_InvalidProvider_ShouldReturnBadRequest()
    {
        // Act
        var result = _controller.ExternalLogin("invalid-provider");

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }
}

// Helper interface for mocking
public interface IChallengeResultWrapper { }
