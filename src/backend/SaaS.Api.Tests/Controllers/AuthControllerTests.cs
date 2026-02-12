using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SaaS.Api.Controllers;
using SaaS.Api.Tests.Helpers;
using SaaS.Application.DTOs;
using SaaS.Application.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Hosting;
using SaaS.Domain.Entities;
using Xunit;

namespace SaaS.Api.Tests.Controllers;

public class AuthControllerTests
{
    private readonly Mock<IAuthService> _mockAuthService;
    private readonly Mock<IWebHostEnvironment> _mockEnv;
    private readonly AuthController _controller;
    private readonly TestDataBuilder _dataBuilder;

    public AuthControllerTests()
    {
        _mockAuthService = new Mock<IAuthService>();
        _mockEnv = new Mock<IWebHostEnvironment>();
        _controller = new AuthController(_mockAuthService.Object, _mockEnv.Object);
        _dataBuilder = new TestDataBuilder();

        // Setup HttpContext
        var httpContext = new DefaultHttpContext();
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsOk()
    {
        // Arrange
        var request = _dataBuilder.Create<LoginRequest>();
        var response = _dataBuilder.Create<AuthResponse>();
        _mockAuthService.Setup(s => s.LoginAsync(request)).ReturnsAsync(response);

        // Act
        var result = await _controller.Login(request);

        // Assert
        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(response);
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
    {
        // Arrange
        var request = _dataBuilder.Create<LoginRequest>();
        _mockAuthService.Setup(s => s.LoginAsync(request)).ThrowsAsync(new Exception("Invalid credentials"));

        // Act
        var result = await _controller.Login(request);

        // Assert
        result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    [Fact]
    public async Task Register_WithValidData_ReturnsOk()
    {
        // Arrange
        var request = _dataBuilder.Create<RegisterRequest>();
        var response = _dataBuilder.Create<AuthResponse>();
        _mockAuthService.Setup(s => s.RegisterAsync(request)).ReturnsAsync(response);

        // Act
        var result = await _controller.Register(request);

        // Assert
        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(response);
    }

    [Fact]
    public async Task Register_WithDuplicateEmail_ReturnsBadRequest()
    {
        // Arrange
        var request = _dataBuilder.Create<RegisterRequest>();
        _mockAuthService.Setup(s => s.RegisterAsync(request)).ThrowsAsync(new Exception("Email already exists"));

        // Act
        var result = await _controller.Register(request);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Theory]
    [InlineData("google")]
    [InlineData("facebook")]
    public void ExternalLogin_ValidProvider_ReturnsChallenge(string provider)
    {
        // Arrange
        var mockUrlHelper = new Mock<IUrlHelper>();
        mockUrlHelper.Setup(x => x.Action(It.IsAny<UrlActionContext>())).Returns("callback-url");
        _controller.Url = mockUrlHelper.Object;

        // Act
        var result = _controller.ExternalLogin(provider);

        // Assert
        result.Should().BeOfType<ChallengeResult>();
        var challengeResult = result as ChallengeResult;
        challengeResult.AuthenticationSchemes.Should().ContainSingle();
    }

    [Fact]
    public void ExternalLogin_InvalidProvider_ReturnsBadRequest()
    {
        // Act
        var result = _controller.ExternalLogin("unsupported");

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task ExternalLoginCallback_Successful_ReturnsRedirect()
    {
        // Arrange
        var email = "test@example.com";
        var name = "Test User";
        var token = "jwt-token";
        var redirectUrl = "http://frontend/callback";

        var claims = new[] 
        { 
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Name, name)
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);
        var authTicket = new AuthenticationTicket(principal, "TestScheme");
        var authResult = AuthenticateResult.Success(authTicket);

        var mockAuthService = new Mock<IAuthenticationService>();
        mockAuthService.Setup(s => s.AuthenticateAsync(It.IsAny<HttpContext>(), It.IsAny<string>()))
            .ReturnsAsync(authResult);

        var serviceProvider = new Mock<IServiceProvider>();
        serviceProvider.Setup(s => s.GetService(typeof(IAuthenticationService)))
            .Returns(mockAuthService.Object);
        
        _controller.HttpContext.RequestServices = serviceProvider.Object;

        var user = new User { Email = email, Username = name };
        _mockAuthService.Setup(s => s.FindOrRegisterExternalUserAsync(email, It.IsAny<string>()))
            .ReturnsAsync(user);
        _mockAuthService.Setup(s => s.GenerateTokenForUser(user)).Returns(token);
        _mockAuthService.Setup(s => s.GetFrontendRedirectUrl(token, email)).Returns(redirectUrl);

        // Act
        var result = await _controller.ExternalLoginCallback();

        // Assert
        result.Should().BeOfType<RedirectResult>();
        (result as RedirectResult).Url.Should().Be(redirectUrl);
    }

    [Fact]
    public async Task ExternalLoginCallback_AuthFailed_ReturnsBadRequest()
    {
        // Arrange
        var authResult = AuthenticateResult.Fail("failed");

        var mockAuthService = new Mock<IAuthenticationService>();
        mockAuthService.Setup(s => s.AuthenticateAsync(It.IsAny<HttpContext>(), It.IsAny<string>()))
            .ReturnsAsync(authResult);

        var serviceProvider = new Mock<IServiceProvider>();
        serviceProvider.Setup(s => s.GetService(typeof(IAuthenticationService)))
            .Returns(mockAuthService.Object);
        
        _controller.HttpContext.RequestServices = serviceProvider.Object;

        // Act
        var result = await _controller.ExternalLoginCallback();

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }
}
