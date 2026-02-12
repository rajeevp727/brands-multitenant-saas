using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SaaS.Domain.Entities;
using SaaS.Infrastructure.Persistence;
using SaaS.Infrastructure.Repositories;
using SaaS.Domain.Interfaces;
using Moq;
using Xunit;

namespace SaaS.Api.Tests.Repositories;

public class RepositoryTests
{
    private readonly ApplicationDbContext _context;
    private readonly UnitOfWork _unitOfWork;

    public RepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var mockTenantProvider = new Mock<ITenantProvider>();
        mockTenantProvider.Setup(p => p.GetTenantId()).Returns("test-tenant");

        _context = new ApplicationDbContext(options, mockTenantProvider.Object);
        _unitOfWork = new UnitOfWork(_context);
    }

    [Fact]
    public async Task GenericRepository_Add_And_GetById_Works()
    {
        // Arrange
        var repo = _unitOfWork.Repository<Product>();
        var product = new Product { Name = "Test Product" };

        // Act
        await repo.AddAsync(product);
        await _unitOfWork.SaveChangesAsync();
        var result = await repo.GetByIdAsync(product.Id);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Test Product");
    }

    [Fact]
    public async Task GenericRepository_GetAll_Works()
    {
        // Arrange
        var repo = _unitOfWork.Repository<Product>();
        _context.Products.AddRange(new List<Product> 
        { 
            new Product { Name = "P1" }, 
            new Product { Name = "P2" } 
        });
        await _context.SaveChangesAsync();

        // Act
        var result = await repo.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GenericRepository_Update_Works()
    {
        // Arrange
        var repo = _unitOfWork.Repository<Product>();
        var product = new Product { Name = "Old Name" };
        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        // Act
        product.Name = "New Name";
        repo.Update(product);
        await _unitOfWork.SaveChangesAsync();

        // Assert
        var updated = await repo.GetByIdAsync(product.Id);
        updated.Name.Should().Be("New Name");
    }

    [Fact]
    public async Task GenericRepository_Delete_Works()
    {
        // Arrange
        var repo = _unitOfWork.Repository<Product>();
        var product = new Product { Name = "Delete Me" };
        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        // Act
        repo.Delete(product);
        await _unitOfWork.SaveChangesAsync();

        // Assert
        var result = await repo.GetByIdAsync(product.Id);
        result.Should().BeNull();
    }

    [Fact]
    public async Task GenericRepository_FindAsync_Works()
    {
        // Arrange
        var repo = _unitOfWork.Repository<Product>();
        _context.Products.Add(new Product { Name = "Target" });
        await _context.SaveChangesAsync();

        // Act
        var result = await repo.FindAsync(p => p.Name == "Target");

        // Assert
        result.Should().ContainSingle();
    }
}
