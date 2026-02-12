using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SaaS.Domain.Entities;
using SaaS.Infrastructure.Persistence;
using SaaS.Infrastructure.Services;
using SaaS.Domain.Interfaces;
using Moq;
using Xunit;

namespace SaaS.Api.Tests.Services;

public class NotificationServiceTests
{
    private readonly ApplicationDbContext _context;
    private readonly NotificationService _service;

    public NotificationServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var mockTenantProvider = new Mock<ITenantProvider>();
        mockTenantProvider.Setup(p => p.GetTenantId()).Returns("test-tenant");

        _context = new ApplicationDbContext(options, mockTenantProvider.Object);
        _service = new NotificationService(_context);
    }

    [Fact]
    public async Task CreateNotificationAsync_SavesToDatabase()
    {
        // Arrange
        var notification = new Notification 
        { 
            Title = "Test", 
            Message = "Msg", 
            TargetRole = "All" 
        };

        // Act
        var result = await _service.CreateNotificationAsync(notification);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be("Test");
        _context.Notifications.Count().Should().Be(1);
    }

    [Fact]
    public async Task GetNotificationsAsync_FiltersByTenant()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var otherTenantId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _context.Notifications.AddRange(new List<Notification>
        {
            new Notification { Title = "T1", TenantId = tenantId, TargetRole = "All", CreatedAt = DateTime.UtcNow },
            new Notification { Title = "T2", TenantId = otherTenantId, TargetRole = "All", CreatedAt = DateTime.UtcNow },
            new Notification { Title = "Global", TenantId = null, TargetRole = "All", CreatedAt = DateTime.UtcNow }
        });
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetNotificationsAsync(userId, "User", tenantId, false);

        // Assert
        result.Should().HaveCount(2); // Own tenant + Global
        result.Should().Contain(n => n.Title == "T1");
        result.Should().Contain(n => n.Title == "Global");
    }

    [Fact]
    public async Task GetNotificationsAsync_SuperAdmin_SeesAll()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var otherTenantId = Guid.NewGuid();

        _context.Notifications.AddRange(new List<Notification>
        {
            new Notification { Title = "T1", TenantId = tenantId, TargetRole = "All", CreatedAt = DateTime.UtcNow },
            new Notification { Title = "T2", TenantId = otherTenantId, TargetRole = "All", CreatedAt = DateTime.UtcNow }
        });
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetNotificationsAsync(Guid.Empty, "Admin", null, true);

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task MarkAsReadAsync_UpdatesDatabase()
    {
        // Arrange
        var n = new Notification { Title = "T", IsRead = false };
        _context.Notifications.Add(n);
        await _context.SaveChangesAsync();

        // Act
        await _service.MarkAsReadAsync(n.Id);

        // Assert
        var updated = await _context.Notifications.FindAsync(n.Id);
        updated.IsRead.Should().BeTrue();
    }

    [Fact]
    public async Task GetUnreadCountAsync_ReturnsCorrectCount()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        _context.Notifications.AddRange(new List<Notification>
        {
            new Notification { Title = "U1", TenantId = tenantId, TargetRole = "All", IsRead = false },
            new Notification { Title = "U2", TenantId = tenantId, TargetRole = "All", IsRead = true },
            new Notification { Title = "U3", TenantId = tenantId, UserId = userId, IsRead = false }
        });
        await _context.SaveChangesAsync();

        // Act
        var count = await _service.GetUnreadCountAsync(userId, "User", tenantId);

        // Assert
        count.Should().Be(2);
    }
}
