using SaaS.Api.Controllers;
using SaaS.Application.Common;
using SaaS.Application.Services;
using SaaS.Domain.Entities;
using SaaS.Application.DTOs;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FluentAssertions;
using Xunit;
using Moq;

namespace SaaS.Api.Tests.Controllers;

public class UsersControllerTests
{
    private readonly Mock<IGenericService<UserDto, User>> _mockUserService;
    private readonly UsersController _controller;

    public UsersControllerTests()
    {
        _mockUserService = new Mock<IGenericService<UserDto, User>>();
        _controller = new UsersController(_mockUserService.Object);

        // Setup HttpContext for testing headers and claims
        var httpContext = new DefaultHttpContext();
        _controller.ControllerContext = new ControllerContext()
        {
            HttpContext = httpContext
        };
    }

    [Fact]
    public async Task GetMe_ReturnsOk_WithUserAndTenantInfo()
    {
        // Arrange
        const string tenantId = "test-tenant";
        const string email = "test@example.com";
        var userId = Guid.NewGuid();
        
        _controller.Request.Headers["X-Tenant-Id"] = tenantId;
        
        var claims = new[] 
        { 
            new Claim("userId", userId.ToString()),
            new Claim(ClaimTypes.Email, email), 
            new Claim(ClaimTypes.Name, "Test User") 
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        _controller.HttpContext.User = new ClaimsPrincipal(identity);

        var userDto = new UserDto { Id = userId.ToString(), Email = email, TenantId = tenantId };
        _mockUserService.Setup(s => s.GetByIdAsync(userId)).ReturnsAsync(userDto);

        // Act
        var result = await _controller.GetMe();

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        var returnedUser = okResult.Value as UserDto;
        
        returnedUser.Email.Should().Be(email);
        returnedUser.TenantId.Should().Be(tenantId);
    }

    [Fact]
    public void GetMyOrders_ReturnsOk_WithListOfOrders()
    {
        // Act
        var result = _controller.GetMyOrders();

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        var orders = okResult.Value as IEnumerable<object>;
        orders.Should().NotBeNull();
        orders.Should().HaveCount(2);
    }
}
