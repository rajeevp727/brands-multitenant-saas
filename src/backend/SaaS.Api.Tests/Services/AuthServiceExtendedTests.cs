using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MockQueryable.Moq;
using Moq;
using SaaS.Api.Tests.Helpers;
using SaaS.Application.DTOs;
using SaaS.Application.Services;
using SaaS.Domain.Entities;
using SaaS.Domain.Interfaces;
using Xunit;

namespace SaaS.Api.Tests.Services;

/// <summary>
/// Extended AuthService tests covering admin bootstrap, Google OAuth user creation,
/// and edge cases that were responsible for the reported 401 and 500 errors.
/// </summary>
public class AuthServiceExtendedTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IJwtProvider> _mockJwtProvider;
    private readonly Mock<ILogger<AuthService>> _mockLogger;
    private readonly Mock<IConfiguration> _mockConfig;
    private readonly TestDataBuilder _builder;
    private readonly AuthService _authService;

    public AuthServiceExtendedTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockJwtProvider = new Mock<IJwtProvider>();
        _mockLogger = new Mock<ILogger<AuthService>>();
        _mockConfig = new Mock<IConfiguration>();
        _builder = new TestDataBuilder();

        _authService = new AuthService(
            _mockUnitOfWork.Object,
            _mockJwtProvider.Object,
            _mockLogger.Object,
            _mockConfig.Object
        );
    }

    // ── Bootstrap Admin ──────────────────────────────────────────────────────────

    [Fact]
    public async Task LoginAsync_BootstrapAdmin_ExactCredentials_ReturnsToken()
    {
        // Arrange - no DB call needed; this is the hardcoded bypass
        _mockJwtProvider.Setup(j => j.GenerateToken(It.IsAny<User>())).Returns("bootstrap-token");

        // We need to ensure the mock repo returns empty so EF doesn't interfere
        var emptyUsers = new List<User>().AsQueryable().BuildMock();
        var mockRepo = new Mock<IGenericRepository<User>>();
        mockRepo.Setup(r => r.GetQueryable()).Returns(emptyUsers);
        _mockUnitOfWork.Setup(u => u.Repository<User>()).Returns(mockRepo.Object);

        // Act — exact credentials from AuthService bootstrap check
        var result = await _authService.LoginAsync(new LoginRequest
        {
            Email = "admin@rajeev.com",
            Password = "Pass123"
        });

        // Assert
        result.Should().NotBeNull();
        result.Token.Should().Be("bootstrap-token");
        result.User.Email.Should().Be("admin@rajeev.com");
        result.User.Role.Should().Be("Admin");
        result.User.TenantId.Should().Be("rajeev-pvt");
    }

    [Theory]
    [InlineData("Admin@Rajeev.Com", "Pass123")] // Case-insensitive email
    [InlineData("ADMIN@RAJEEV.COM", "Pass123")] // All caps email
    public async Task LoginAsync_BootstrapAdmin_CaseInsensitiveEmail_ReturnsToken(string email, string password)
    {
        // Arrange
        _mockJwtProvider.Setup(j => j.GenerateToken(It.IsAny<User>())).Returns("token");

        var emptyUsers = new List<User>().AsQueryable().BuildMock();
        var mockRepo = new Mock<IGenericRepository<User>>();
        mockRepo.Setup(r => r.GetQueryable()).Returns(emptyUsers);
        _mockUnitOfWork.Setup(u => u.Repository<User>()).Returns(mockRepo.Object);

        // Act
        var result = await _authService.LoginAsync(new LoginRequest { Email = email, Password = password });

        // Assert
        result.Token.Should().NotBeEmpty("bootstrap admin is case-insensitive on email");
    }

    [Fact]
    public async Task LoginAsync_BootstrapAdmin_WrongPassword_FallsThroughToDb()
    {
        // Arrange — wrong password means bootstrap is skipped, DB lookup runs
        var emptyUsers = new List<User>().AsQueryable().BuildMock();
        var mockRepo = new Mock<IGenericRepository<User>>();
        mockRepo.Setup(r => r.GetQueryable()).Returns(emptyUsers);
        _mockUnitOfWork.Setup(u => u.Repository<User>()).Returns(mockRepo.Object);

        var request = new LoginRequest { Email = "admin@rajeev.com", Password = "WrongPassword" };

        // Act & Assert — should throw because DB has no user with that email after bypass
        await Assert.ThrowsAsync<Exception>(() => _authService.LoginAsync(request));
    }

    // ── Normal DB Login ──────────────────────────────────────────────────────────

    [Fact]
    public async Task LoginAsync_DbUser_ValidPlaintextPassword_Succeeds()
    {
        // Arrange
        var user = _builder.CreateUser("user@example.com");
        user.PasswordHash = "PlainTextPass";
        user.Role = new Role { Name = "User" };

        var users = new List<User> { user }.AsQueryable().BuildMock();
        var mockRepo = new Mock<IGenericRepository<User>>();
        mockRepo.Setup(r => r.GetQueryable()).Returns(users);
        _mockUnitOfWork.Setup(u => u.Repository<User>()).Returns(mockRepo.Object);
        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        _mockJwtProvider.Setup(j => j.GenerateToken(It.IsAny<User>())).Returns("user-token");

        // Act
        var result = await _authService.LoginAsync(new LoginRequest
        {
            Email = "user@example.com",
            Password = "PlainTextPass"
        });

        // Assert
        result.Token.Should().Be("user-token");
        result.User.Email.Should().Be("user@example.com");
    }

    [Fact]
    public async Task LoginAsync_DbUser_SavesRefreshToken()
    {
        // Arrange
        var user = _builder.CreateUser("user@example.com");
        user.PasswordHash = "password123";
        user.Role = new Role { Name = "User" };

        var users = new List<User> { user }.AsQueryable().BuildMock();
        var mockRepo = new Mock<IGenericRepository<User>>();
        mockRepo.Setup(r => r.GetQueryable()).Returns(users);
        _mockUnitOfWork.Setup(u => u.Repository<User>()).Returns(mockRepo.Object);
        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        _mockJwtProvider.Setup(j => j.GenerateToken(It.IsAny<User>())).Returns("token");

        // Act
        var result = await _authService.LoginAsync(new LoginRequest
        {
            Email = "user@example.com",
            Password = "password123"
        });

        // Assert
        result.RefreshToken.Should().NotBeNullOrEmpty("refresh token must be generated on login");
        user.RefreshToken.Should().NotBeNullOrEmpty("refresh token must be persisted to the user entity");
        user.RefreshTokenExpiryTime.Should().BeAfter(DateTime.UtcNow, "expiry must be in future");
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    // ── Google OAuth - FindOrRegisterExternalUser ─────────────────────────────────

    [Fact]
    public async Task FindOrRegisterExternalUserAsync_NewUser_NoRoleFound_StillCreatesUser()
    {
        // Arrange — simulates the "no Customer role seeded" scenario
        var email = "google@example.com";
        var userList = new List<User>();

        var mockUserRepo = new Mock<IGenericRepository<User>>();
        mockUserRepo.Setup(r => r.GetQueryable())
            .Returns(() => userList.AsQueryable().BuildMock());
        mockUserRepo.Setup(r => r.AddAsync(It.IsAny<User>()))
            .Callback<User>(u => { u.Id = Guid.NewGuid(); userList.Add(u); })
            .Returns(Task.CompletedTask);

        // Empty role repo — Customer role not seeded
        var emptyRoles = new List<Role>().AsQueryable().BuildMock();
        var mockRoleRepo = new Mock<IGenericRepository<Role>>();
        mockRoleRepo.Setup(r => r.GetQueryable()).Returns(emptyRoles);

        _mockUnitOfWork.Setup(u => u.Repository<User>()).Returns(mockUserRepo.Object);
        _mockUnitOfWork.Setup(u => u.Repository<Role>()).Returns(mockRoleRepo.Object);
        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act — should NOT throw even without a role
        var result = await _authService.FindOrRegisterExternalUserAsync(email, "Google User");

        // Assert
        result.Should().NotBeNull();
        result.Email.Should().Be(email);
        result.PasswordHash.Should().Be("EXTERNAL_AUTH");
        mockUserRepo.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task FindOrRegisterExternalUserAsync_NewUser_SetsDefaultTenant()
    {
        // Arrange
        var email = "oauth@example.com";
        var userList = new List<User>();

        var mockUserRepo = new Mock<IGenericRepository<User>>();
        mockUserRepo.Setup(r => r.GetQueryable()).Returns(() => userList.AsQueryable().BuildMock());
        mockUserRepo.Setup(r => r.AddAsync(It.IsAny<User>()))
            .Callback<User>(u => { u.Id = Guid.NewGuid(); userList.Add(u); })
            .Returns(Task.CompletedTask);

        var customerRole = new Role { Id = Guid.NewGuid(), Name = "Customer" };
        var roles = new List<Role> { customerRole }.AsQueryable().BuildMock();
        var mockRoleRepo = new Mock<IGenericRepository<Role>>();
        mockRoleRepo.Setup(r => r.GetQueryable()).Returns(roles);

        _mockUnitOfWork.Setup(u => u.Repository<User>()).Returns(mockUserRepo.Object);
        _mockUnitOfWork.Setup(u => u.Repository<Role>()).Returns(mockRoleRepo.Object);
        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        var result = await _authService.FindOrRegisterExternalUserAsync(email, "Test Name");

        // Assert
        result.Should().NotBeNull();
        // The AuthService hardcodes "rajeev-pvt" as the default tenant for OAuth users
        mockUserRepo.Verify(r => r.AddAsync(It.Is<User>(u => u.TenantId == "rajeev-pvt")), Times.Once,
            "OAuth users must be created in the default tenant rajeev-pvt");
    }

    [Fact]
    public async Task FindOrRegisterExternalUserAsync_ExistingUser_DoesNotCreateDuplicate()
    {
        // Arrange
        var email = "existing@example.com";
        var existing = _builder.CreateUser(email);
        existing.Role = new Role { Name = "Customer" };

        var users = new List<User> { existing }.AsQueryable().BuildMock();
        var mockRepo = new Mock<IGenericRepository<User>>();
        mockRepo.Setup(r => r.GetQueryable()).Returns(users);
        _mockUnitOfWork.Setup(u => u.Repository<User>()).Returns(mockRepo.Object);

        // Act
        var result = await _authService.FindOrRegisterExternalUserAsync(email, "Existing User");

        // Assert
        result.Email.Should().Be(email);
        mockRepo.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never,
            "existing users must NOT be re-created");
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task FindOrRegisterExternalUserAsync_EmptyName_UsesEmailPrefix()
    {
        // Arrange
        var email = "noname@example.com";
        var userList = new List<User>();

        var mockUserRepo = new Mock<IGenericRepository<User>>();
        mockUserRepo.Setup(r => r.GetQueryable()).Returns(() => userList.AsQueryable().BuildMock());
        mockUserRepo.Setup(r => r.AddAsync(It.IsAny<User>()))
            .Callback<User>(u => { u.Id = Guid.NewGuid(); userList.Add(u); })
            .Returns(Task.CompletedTask);

        var emptyRoles = new List<Role>().AsQueryable().BuildMock();
        var mockRoleRepo = new Mock<IGenericRepository<Role>>();
        mockRoleRepo.Setup(r => r.GetQueryable()).Returns(emptyRoles);

        _mockUnitOfWork.Setup(u => u.Repository<User>()).Returns(mockUserRepo.Object);
        _mockUnitOfWork.Setup(u => u.Repository<Role>()).Returns(mockRoleRepo.Object);
        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act — empty name string
        var result = await _authService.FindOrRegisterExternalUserAsync(email, "");

        // Assert — username should be "noname" (the part before @)
        mockUserRepo.Verify(r => r.AddAsync(It.Is<User>(u => u.Username == "noname")), Times.Once,
            "empty name must fall back to email prefix");
    }

    // ── Refresh Token ────────────────────────────────────────────────────────────

    [Fact]
    public async Task RefreshTokenAsync_ValidToken_ReturnsNewTokenPair()
    {
        // Arrange
        var refreshToken = "valid-refresh-token";
        var user = _builder.CreateUser();
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        user.Role = new Role { Name = "User" };

        var users = new List<User> { user }.AsQueryable().BuildMock();
        var mockRepo = new Mock<IGenericRepository<User>>();
        mockRepo.Setup(r => r.GetQueryable()).Returns(users);
        _mockUnitOfWork.Setup(u => u.Repository<User>()).Returns(mockRepo.Object);
        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        _mockJwtProvider.Setup(j => j.GenerateToken(It.IsAny<User>())).Returns("new-jwt");

        // Act
        var result = await _authService.RefreshTokenAsync(new RefreshTokenRequest { RefreshToken = refreshToken });

        // Assert
        result.Token.Should().Be("new-jwt");
        result.RefreshToken.Should().NotBe(refreshToken, "refresh token must be rotated");
    }

    [Fact]
    public async Task RefreshTokenAsync_ExpiredToken_ThrowsException()
    {
        // Arrange
        var user = _builder.CreateUser();
        user.RefreshToken = "expired-token";
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(-1); // expired

        var users = new List<User> { user }.AsQueryable().BuildMock();
        var mockRepo = new Mock<IGenericRepository<User>>();
        mockRepo.Setup(r => r.GetQueryable()).Returns(users);
        _mockUnitOfWork.Setup(u => u.Repository<User>()).Returns(mockRepo.Object);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() =>
            _authService.RefreshTokenAsync(new RefreshTokenRequest { RefreshToken = "expired-token" }));
    }

    [Fact]
    public async Task RefreshTokenAsync_UnknownToken_ThrowsException()
    {
        // Arrange
        var emptyUsers = new List<User>().AsQueryable().BuildMock();
        var mockRepo = new Mock<IGenericRepository<User>>();
        mockRepo.Setup(r => r.GetQueryable()).Returns(emptyUsers);
        _mockUnitOfWork.Setup(u => u.Repository<User>()).Returns(mockRepo.Object);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() =>
            _authService.RefreshTokenAsync(new RefreshTokenRequest { RefreshToken = "unknown-token" }));
    }

    // ── Frontend Redirect URL ────────────────────────────────────────────────────

    [Fact]
    public void GetFrontendRedirectUrl_ReturnsFrontendCallbackUrl()
    {
        // Arrange
        var email = "user@test.com";
        var token = "jwt-token";

        // Act
        var url = _authService.GetFrontendRedirectUrl(token, email);

        // Assert
        url.Should().Contain("/auth/callback");
        url.Should().Contain(email);
    }

    [Fact]
    public void GetFrontendRedirectUrl_UsesFrontendUrlFromConfig()
    {
        // Arrange
        _mockConfig.Setup(c => c["FrontendUrl"]).Returns("https://my-app.com");

        // Act
        var url = _authService.GetFrontendRedirectUrl("tok", "user@test.com");

        // Assert
        url.Should().StartWith("https://my-app.com", "FrontendUrl config must be used when set");
    }
}
