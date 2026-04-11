using AutoFixture;
using Moq;
using SaaS.Domain.Entities;
using SaaS.Domain.Interfaces;
using SaaS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace SaaS.Api.Tests.Helpers;

public class TestDataBuilder
{
    private readonly Fixture _fixture;

    public TestDataBuilder()
    {
        _fixture = new Fixture();
        // Prevent circular references
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    public User CreateUser(string? email = null, string? tenantId = null)
    {
        return _fixture.Build<User>()
            .With(u => u.Email, email ?? _fixture.Create<string>() + "@example.com")
            .With(u => u.TenantId, tenantId ?? "rajeev-pvt")
            .Without(u => u.Role)
            .Without(u => u.Orders)
            .Create();
    }

    public Role CreateRole(string name = "Customer", string tenantId = "rajeev-pvt")
    {
        return new Role
        {
            Id = Guid.NewGuid(),
            Name = name,
            TenantId = tenantId
        };
    }

    public Brand CreateBrand()
    {
        return _fixture.Build<Brand>()
            .Create();
    }

    /// <summary>Creates an in-memory ApplicationDbContext wired with a mock tenant provider.</summary>
    public static ApplicationDbContext CreateInMemoryContext(string? tenantId = "rajeev-pvt")
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var mockTenantProvider = new Mock<ITenantProvider>();
        mockTenantProvider.Setup(p => p.GetTenantId()).Returns(tenantId);

        var mockUserContext = new Mock<IUserContext>();
        mockUserContext.Setup(u => u.UserId).Returns("test-user");

        return new ApplicationDbContext(options, mockTenantProvider.Object, mockUserContext.Object);
    }

    public T Create<T>() => _fixture.Create<T>();
    public IEnumerable<T> CreateMany<T>(int count = 3) => _fixture.CreateMany<T>(count);
}
