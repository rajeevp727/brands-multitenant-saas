using Moq;
using Xunit;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using SaaS.Infrastructure.Auth;
using SaaS.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SaaS.Api.Tests.Infrastructure;

public class JwtProviderTests
{
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly JwtProvider _jwtProvider;

    public JwtProviderTests()
    {
        _mockConfiguration = new Mock<IConfiguration>();
        _mockConfiguration.Setup(c => c["Jwt:Key"]).Returns("super_secret_key_1234567890_antigravity_saas_min_32_chars");
        _mockConfiguration.Setup(c => c["Jwt:Issuer"]).Returns("test-issuer");
        _mockConfiguration.Setup(c => c["Jwt:Audience"]).Returns("test-audience");

        _jwtProvider = new JwtProvider(_mockConfiguration.Object);
    }

    [Fact]
    public void GenerateToken_ValidUser_ReturnsTokenWithClaims()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            Username = "testuser",
            TenantId = "tenant-1",
            Role = new Role { Name = "Admin" }
        };

        // Act
        var tokenString = _jwtProvider.GenerateToken(user);

        // Assert
        tokenString.Should().NotBeNullOrEmpty();

        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(tokenString);

        token.Issuer.Should().Be("test-issuer");
        token.Audiences.Should().Contain("test-audience");
        
        token.Claims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value.Should().Be(user.Id.ToString());
        token.Claims.First(c => c.Type == JwtRegisteredClaimNames.Email).Value.Should().Be(user.Email);
        token.Claims.First(c => c.Type == "userId").Value.Should().Be(user.Id.ToString());
        token.Claims.First(c => c.Type == "username").Value.Should().Be(user.Username);
        token.Claims.First(c => c.Type == "tenantId").Value.Should().Be(user.TenantId);
        token.Claims.First(c => c.Type == ClaimTypes.Role).Value.Should().Be("Admin");
    }
    
    [Fact]
    public void GenerateToken_UserWithoutRole_DefaultsToUserRole()
    {
         // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "no-role@example.com",
            Username = "norole",
            TenantId = "tenant-1",
            Role = null
        };

        // Act
        var tokenString = _jwtProvider.GenerateToken(user);

        // Assert
        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(tokenString);
        
        token.Claims.First(c => c.Type == ClaimTypes.Role).Value.Should().Be("User");
    }
}
