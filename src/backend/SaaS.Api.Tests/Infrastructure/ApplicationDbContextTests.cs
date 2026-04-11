using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using SaaS.Api.Tests.Helpers;
using SaaS.Domain.Entities;
using SaaS.Domain.Interfaces;
using SaaS.Infrastructure.Persistence;
using Xunit;

namespace SaaS.Api.Tests.Infrastructure;

/// <summary>
/// Tests for ApplicationDbContext tenant security enforcement and fallback behavior.
/// These cover the core bug-fixes: null tenant fallback and cross-tenant protection.
/// </summary>
public class ApplicationDbContextTests
{
    // ── Tenant Fallback ──────────────────────────────────────────────────────────

    [Fact]
    public async Task SaveChangesAsync_WithNullTenant_FallsBackToDefaultTenant()
    {
        // Arrange - simulate auth flow with no X-Tenant-Id header
        using var context = TestDataBuilder.CreateInMemoryContext(tenantId: null);

        var brand = new Brand
        {
            Id = Guid.NewGuid(),
            Name = "Test Brand",
            TenantId = "should-be-overwritten"
        };
        context.Brands.Add(brand);

        // Act — should NOT throw even with null tenant
        var act = async () => await context.SaveChangesAsync();

        // Assert
        await act.Should().NotThrowAsync("null tenant should fall back to rajeev-pvt");
        brand.TenantId.Should().Be("rajeev-pvt", "null tenant falls back to default");
    }

    [Fact]
    public async Task SaveChangesAsync_WithValidTenant_StampsTenantIdOnAddedEntity()
    {
        // Arrange
        using var context = TestDataBuilder.CreateInMemoryContext("omega");

        var brand = new Brand
        {
            Id = Guid.NewGuid(),
            Name = "Omega Brand",
            TenantId = "wrong-tenant" // Should be overwritten
        };
        context.Brands.Add(brand);

        // Act
        await context.SaveChangesAsync();

        // Assert
        brand.TenantId.Should().Be("omega", "SaveChanges must overwrite TenantId with the active tenant");
    }

    [Fact]
    public async Task SaveChangesAsync_CrossTenantDelete_ThrowsUnauthorized()
    {
        // Arrange - add an entity as tenant-A
        using var contextA = TestDataBuilder.CreateInMemoryContext("tenant-a");
        var brand = new Brand { Id = Guid.NewGuid(), Name = "Tenant A Brand", TenantId = "tenant-a" };
        contextA.Brands.Add(brand);
        await contextA.SaveChangesAsync();

        // Now try to delete it as tenant-B (different context, same DB — simulated)
        // We simulate by marking it Deleted in a new context for tenant-b
        using var contextB = TestDataBuilder.CreateInMemoryContext("tenant-b");
        var alienBrand = new Brand { Id = brand.Id, Name = brand.Name, TenantId = "tenant-a" };
        contextB.Entry(alienBrand).State = EntityState.Deleted;

        // Act
        var act = async () => await contextB.SaveChangesAsync();

        // Assert — cross-tenant delete must be blocked
        await act.Should().ThrowAsync<UnauthorizedAccessException>("cross-tenant deletes must be blocked");
    }

    [Fact]
    public async Task SaveChangesAsync_ModifyEntity_PreventsTenantSwitching()
    {
        // Arrange
        using var context = TestDataBuilder.CreateInMemoryContext("rajeev-pvt");
        var brand = new Brand { Id = Guid.NewGuid(), Name = "Brand", TenantId = "rajeev-pvt" };
        context.Brands.Add(brand);
        await context.SaveChangesAsync();

        // Try to switch tenant on an update
        brand.TenantId = "hacked-tenant";
        context.Entry(brand).State = EntityState.Modified;

        // Act
        await context.SaveChangesAsync();

        // Assert — TenantId must NOT be modified
        var savedBrand = await context.Brands.IgnoreQueryFilters().FirstAsync(b => b.Id == brand.Id);
        savedBrand.TenantId.Should().Be("rajeev-pvt", "TenantId is immutable on update");
    }

    // ── Tenant Query Filter ───────────────────────────────────────────────────────

    [Fact]
    public async Task QueryFilter_OnlyReturnsBrandsForCurrentTenant()
    {
        // Arrange - seed two brands for different tenants
        using var seedCtx = TestDataBuilder.CreateInMemoryContext("tenant-a");
        seedCtx.Brands.Add(new Brand { Id = Guid.NewGuid(), Name = "Brand A", TenantId = "tenant-a" });
        await seedCtx.SaveChangesAsync();

        using var seedCtx2 = TestDataBuilder.CreateInMemoryContext("tenant-b");
        seedCtx2.Brands.Add(new Brand { Id = Guid.NewGuid(), Name = "Brand B", TenantId = "tenant-b" });
        await seedCtx2.SaveChangesAsync();

        // Separate read context for tenant-a
        // In-memory DB is per-instance here; just verify filter is applied correctly
        using var readCtx = TestDataBuilder.CreateInMemoryContext("tenant-a");
        readCtx.Brands.Add(new Brand { Id = Guid.NewGuid(), Name = "Visible", TenantId = "tenant-a" });
        readCtx.Brands.Add(new Brand { Id = Guid.NewGuid(), Name = "Hidden", TenantId = "tenant-b" });
        await readCtx.SaveChangesAsync();

        // Act — filtered query
        var brands = await readCtx.Brands.ToListAsync();

        // Assert
        brands.Should().AllSatisfy(b => b.TenantId.Should().Be("tenant-a"));
        brands.Should().NotContain(b => b.Name == "Hidden");
    }

    [Fact]
    public async Task IgnoreQueryFilters_ReturnsAllTenantBrands()
    {
        // Arrange
        using var ctx = TestDataBuilder.CreateInMemoryContext("tenant-a");
        ctx.Brands.Add(new Brand { Id = Guid.NewGuid(), Name = "Brand A", TenantId = "tenant-a" });
        ctx.Brands.Add(new Brand { Id = Guid.NewGuid(), Name = "Brand B", TenantId = "tenant-b" });
        await ctx.SaveChangesAsync();

        // Act
        var all = await ctx.Brands.IgnoreQueryFilters().ToListAsync();

        // Assert
        all.Should().HaveCount(2, "IgnoreQueryFilters bypasses tenant filter");
    }
}
