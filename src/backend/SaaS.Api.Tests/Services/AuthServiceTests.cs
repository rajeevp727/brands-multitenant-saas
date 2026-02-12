using Moq;
using SaaS.Application.DTOs;
using SaaS.Application.Services;
using SaaS.Domain.Entities;
using SaaS.Domain.Interfaces;
using SaaS.Api.Tests.Helpers;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;

namespace SaaS.Api.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IJwtProvider> _mockJwtProvider;
    private readonly Mock<ILogger<AuthService>> _mockLogger;
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly TestDataBuilder _dataBuilder;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockJwtProvider = new Mock<IJwtProvider>();
        _mockLogger = new Mock<ILogger<AuthService>>();
        _mockConfiguration = new Mock<IConfiguration>();
        _dataBuilder = new TestDataBuilder();

        _authService = new AuthService(
            _mockUnitOfWork.Object,
            _mockJwtProvider.Object,
            _mockLogger.Object,
            _mockConfiguration.Object
        );
    }

    [Fact]
    public async Task LoginAsync_ValidCredentials_ReturnsAuthResponse()
    {
        // Arrange
        var password = "PlaintextPassword123";
        var user = _dataBuilder.CreateUser("test@example.com");
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
        user.Role = new Role { Name = "Admin" };

        var users = new List<User> { user }.AsQueryable().BuildMock();
        
        var mockRepo = new Mock<IGenericRepository<User>>();
        mockRepo.Setup(r => r.GetQueryable()).Returns(users);
        _mockUnitOfWork.Setup(u => u.Repository<User>()).Returns(mockRepo.Object);
        _mockJwtProvider.Setup(j => j.GenerateToken(It.IsAny<User>())).Returns("test-token");

        var request = new LoginRequest { Email = user.Email, Password = password };

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Token.Should().Be("test-token");
        result.User.Email.Should().Be(user.Email);
        result.User.Role.Should().Be("Admin");
    }

    [Fact]
    public async Task LoginAsync_InvalidUser_ThrowsException()
    {
        // Arrange
        var users = new List<User>().AsQueryable().BuildMock();
        var mockRepo = new Mock<IGenericRepository<User>>();
        mockRepo.Setup(r => r.GetQueryable()).Returns(users);
        _mockUnitOfWork.Setup(u => u.Repository<User>()).Returns(mockRepo.Object);

        var request = new LoginRequest { Email = "nonexistent@example.com", Password = "password" };

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _authService.LoginAsync(request));
    }

    [Fact]
    public async Task LoginAsync_FallbackAuth_Success()
    {
        // Arrange
        var password = "PlaintextPassword123";
        var user = _dataBuilder.CreateUser("test@example.com");
        user.PasswordHash = password; // Set hash to plaintext for fallback
        user.Role = new Role { Name = "Admin" };

        var users = new List<User> { user }.AsQueryable().BuildMock();
        
        var mockRepo = new Mock<IGenericRepository<User>>();
        mockRepo.Setup(r => r.GetQueryable()).Returns(users);
        _mockUnitOfWork.Setup(u => u.Repository<User>()).Returns(mockRepo.Object);
        _mockJwtProvider.Setup(j => j.GenerateToken(It.IsAny<User>())).Returns("test-token");

        var request = new LoginRequest { Email = user.Email, Password = password };

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Token.Should().Be("test-token");
    }

    [Fact]
    public async Task LoginAsync_InvalidPassword_ThrowsException()
    {
        // Arrange
        var user = _dataBuilder.CreateUser("test@example.com");
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword("CorrectPassword");
        
        var users = new List<User> { user }.AsQueryable().BuildMock();
        var mockRepo = new Mock<IGenericRepository<User>>();
        mockRepo.Setup(r => r.GetQueryable()).Returns(users);
        _mockUnitOfWork.Setup(u => u.Repository<User>()).Returns(mockRepo.Object);

        var request = new LoginRequest { Email = user.Email, Password = "WrongPassword" };

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _authService.LoginAsync(request));
    }

    [Fact]
    public async Task RegisterAsync_NewUser_ReturnsAuthResponse()
    {
        // Arrange
        var request = new RegisterRequest 
        { 
            Email = "new@example.com", 
            Username = "newuser", 
            Password = "password",
            RoleName = "Customer"
        };

        var users = new List<User>().AsQueryable().BuildMock();
        var roles = new List<Role> { new Role { Id = Guid.NewGuid(), Name = "Customer" } }.AsQueryable().BuildMock();

        var mockUserRepo = new Mock<IGenericRepository<User>>();
        mockUserRepo.Setup(r => r.GetQueryable()).Returns(users);
        
        var mockRoleRepo = new Mock<IGenericRepository<Role>>();
        mockRoleRepo.Setup(r => r.GetQueryable()).Returns(roles);

        _mockUnitOfWork.Setup(u => u.Repository<User>()).Returns(mockUserRepo.Object);
        _mockUnitOfWork.Setup(u => u.Repository<Role>()).Returns(mockRoleRepo.Object);
        _mockJwtProvider.Setup(j => j.GenerateToken(It.IsAny<User>())).Returns("test-token");

        // Act
        var result = await _authService.RegisterAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Token.Should().Be("test-token");
        result.User.Email.Should().Be(request.Email);
        mockUserRepo.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Exactly(1));
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(1));
    }

    [Fact]
    public async Task RegisterAsync_ExistingUser_ThrowsException()
    {
        // Arrange
        var request = new RegisterRequest { Email = "existing@example.com", Username = "user", Password = "password" };
        var user = _dataBuilder.CreateUser(request.Email);
        
        var users = new List<User> { user }.AsQueryable().BuildMock();
        var mockRepo = new Mock<IGenericRepository<User>>();
        mockRepo.Setup(r => r.GetQueryable()).Returns(users);
        _mockUnitOfWork.Setup(u => u.Repository<User>()).Returns(mockRepo.Object);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _authService.RegisterAsync(request));
    }

    [Fact]
    public async Task RegisterAsync_RoleNotFound_FallsBackToDefault()
    {
        // Arrange
        var request = new RegisterRequest { Email = "new@example.com", Username = "newuser", Password = "password", RoleName = "NonExistent" };
        var defaultRole = new Role { Id = Guid.NewGuid(), Name = "Default" };

        var users = new List<User>().AsQueryable().BuildMock();
        var roles = new List<Role> { defaultRole }.AsQueryable().BuildMock(); // Only default role exists

        var mockUserRepo = new Mock<IGenericRepository<User>>();
        mockUserRepo.Setup(r => r.GetQueryable()).Returns(users);
        
        var mockRoleRepo = new Mock<IGenericRepository<Role>>();
        mockRoleRepo.Setup(r => r.GetQueryable()).Returns(roles);

        _mockUnitOfWork.Setup(u => u.Repository<User>()).Returns(mockUserRepo.Object);
        _mockUnitOfWork.Setup(u => u.Repository<Role>()).Returns(mockRoleRepo.Object);
        _mockJwtProvider.Setup(j => j.GenerateToken(It.IsAny<User>())).Returns("test-token");

        // Act
        var result = await _authService.RegisterAsync(request);

        // Assert
        result.User.Role.Should().Be("Default");
    }

    [Fact]
    public async Task FindOrRegisterExternalUserAsync_ExistingUser_ReturnsUser()
    {
        // Arrange
        var email = "external@example.com";
        var user = _dataBuilder.CreateUser(email);
        user.Role = new Role { Name = "Customer" };

        var users = new List<User> { user }.AsQueryable().BuildMock();
        var mockRepo = new Mock<IGenericRepository<User>>();
        mockRepo.Setup(r => r.GetQueryable()).Returns(users);
        _mockUnitOfWork.Setup(u => u.Repository<User>()).Returns(mockRepo.Object);

        // Act
        var result = await _authService.FindOrRegisterExternalUserAsync(email, "External User");

        // Assert
        result.Should().NotBeNull();
        result.Email.Should().Be(email);
        mockRepo.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never());
    }

    [Fact]
    public async Task FindOrRegisterExternalUserAsync_NewUser_CreatesUser()
    {
        // Arrange
        var email = "new.external@example.com";
        var name = "New External User";
        var customerRole = new Role { Id = Guid.NewGuid(), Name = "Customer" };

        var users = new List<User>().AsQueryable().BuildMock(); // Start empty
        // Use a list to simulate state changes
        var userList = new List<User>();
        
        var mockUserRepo = new Mock<IGenericRepository<User>>();
        mockUserRepo.Setup(r => r.GetQueryable()).Returns(() => userList.AsQueryable().BuildMock());
        
        mockUserRepo.Setup(r => r.AddAsync(It.IsAny<User>()))
            .Callback<User>(u => {
                u.Id = Guid.NewGuid();
                u.Role = customerRole;
                userList.Add(u);
            })
            .Returns(Task.CompletedTask);

        var roles = new List<Role> { customerRole }.AsQueryable().BuildMock();
        var mockRoleRepo = new Mock<IGenericRepository<Role>>();
        mockRoleRepo.Setup(r => r.GetQueryable()).Returns(roles);

        _mockUnitOfWork.Setup(u => u.Repository<User>()).Returns(mockUserRepo.Object);
        _mockUnitOfWork.Setup(u => u.Repository<Role>()).Returns(mockRoleRepo.Object);

        // Act
        var result = await _authService.FindOrRegisterExternalUserAsync(email, name);

        // Assert
        result.Should().NotBeNull();
        result.Email.Should().Be(email);
        result.Role.Name.Should().Be("Customer");
        mockUserRepo.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once());
    }

    [Fact]
    public void GenerateTokenForUser_CallsJwtProvider()
    {
        // Arrange
        var user = new User { Id = Guid.NewGuid(), Email = "test@test.com" };
        _mockJwtProvider.Setup(j => j.GenerateToken(user)).Returns("token");

        // Act
        var result = _authService.GenerateTokenForUser(user);

        // Assert
        result.Should().Be("token");
        _mockJwtProvider.Verify(j => j.GenerateToken(user), Times.Once);
    }

    [Fact]
    public void GetFrontendRedirectUrl_ReturnsCorrectUrl()
    {
        // Arrange
        var token = "token123";
        var email = "test@test.com";

        // Act
        var result = _authService.GetFrontendRedirectUrl(token, email);

        // Assert
        result.Should().Be($"https://rajeevs-pvt-ltd.vercel.app/auth/callback?token={token}&email={email}");
    }
}
