using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using SaaS.Api.Middleware;
using SaaS.Api.Services;
using SaaS.Domain.Interfaces;
using Xunit;
using System.Security.Claims;

namespace SaaS.Api.Tests.Middleware;

public class MiddlewareTests
{
    [Fact]
    public async Task TenantMiddleware_InvokesNext_AndSetsTenantIdInItems()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var mockTenantProvider = new Mock<ITenantProvider>();
        mockTenantProvider.Setup(p => p.GetTenantId()).Returns("test-tenant");

        RequestDelegate next = (innerContext) => Task.CompletedTask;
        var middleware = new TenantMiddleware(next);

        // Act
        await middleware.InvokeAsync(context, mockTenantProvider.Object);

        // Assert
        context.Items["TenantId"].Should().Be("test-tenant");
    }

    [Fact]
    public void HttpContextTenantProvider_GetTenantId_FromHeader_ReturnsTenantId()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Headers["X-Tenant-Id"] = "header-tenant";
        
        var mockAccessor = new Mock<IHttpContextAccessor>();
        mockAccessor.Setup(a => a.HttpContext).Returns(context);

        var provider = new HttpContextTenantProvider(mockAccessor.Object);

        // Act
        var result = provider.GetTenantId();

        // Assert
        result.Should().Be("header-tenant");
    }

    [Fact]
    public void HttpContextTenantProvider_GetTenantId_FromHost_ReturnsHost()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Host = new HostString("test.rajeevsTech.in");
        
        var mockAccessor = new Mock<IHttpContextAccessor>();
        mockAccessor.Setup(a => a.HttpContext).Returns(context);

        var provider = new HttpContextTenantProvider(mockAccessor.Object);

        // Act
        var result = provider.GetTenantId();

        // Assert
        result.Should().Be("test.rajeevsTech.in");
    }

    [Fact]
    public void HttpContextTenantProvider_GetTenantId_NoContext_ReturnsNull()
    {
        // Arrange
        var mockAccessor = new Mock<IHttpContextAccessor>();
        mockAccessor.Setup(a => a.HttpContext).Returns((HttpContext)null);

        var provider = new HttpContextTenantProvider(mockAccessor.Object);

        // Act
        var result = provider.GetTenantId();

        // Assert
        result.Should().BeNull();
    }
}
