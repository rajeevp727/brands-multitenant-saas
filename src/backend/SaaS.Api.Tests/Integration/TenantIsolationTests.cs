using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SaaS.Domain.Entities;
using SaaS.Infrastructure.Persistence;
using System.Net.Http.Headers;
using Xunit;
using FluentAssertions;
using System.Collections.Generic;
using System.Net.Http.Json;
using SaaS.Application.DTOs;

namespace SaaS.Api.Tests.Integration;

public class TenantIsolationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public TenantIsolationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                if (descriptor != null) services.Remove(descriptor);

                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TenantIsolationTestDb");
                });
            });
        });

        SeedData();
    }

    private void SeedData()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        // Seed Tenants
        context.Tenants.AddRange(
            new Tenant { Id = "tenant1", Name = "Tenant 1" },
            new Tenant { Id = "tenant2", Name = "Tenant 2" }
        );

        // Seed Products for different tenants
        context.Products.AddRange(
            new Product { Name = "Product T1", TenantId = "tenant1", Price = 10 },
            new Product { Name = "Product T2", TenantId = "tenant2", Price = 20 }
        );

        // Seed Brands
        context.Brands.AddRange(
            new Brand { Name = "Brand 1", TenantId = "tenant1", IsVisible = true },
            new Brand { Name = "Brand 2", TenantId = "tenant2", IsVisible = true }
        );

        context.SaveChanges();
    }

    [Fact]
    public async Task GetProducts_ReturnsOnlyTenantSpecificProducts()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Tenant-Id", "tenant1");

        // Act
        var response = await client.GetAsync("/api/products");

        // Assert
        response.EnsureSuccessStatusCode();
        var products = await response.Content.ReadFromJsonAsync<List<ProductDto>>();
        products.Should().NotBeNull();
        products.Should().HaveCount(1);
        products![0].Name.Should().Be("Product T1");
    }

    [Fact]
    public async Task GetProducts_OtherTenant_ReturnsOnlyTenantSpecificProducts()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Tenant-Id", "tenant2");

        // Act
        var response = await client.GetAsync("/api/products");

        // Assert
        response.EnsureSuccessStatusCode();
        var products = await response.Content.ReadFromJsonAsync<List<ProductDto>>();
        products.Should().NotBeNull();
        products.Should().HaveCount(1);
        products![0].Name.Should().Be("Product T2");
    }

    [Fact]
    public async Task GetCurrentBrand_ReturnsCorrectBrand()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Tenant-Id", "tenant1");

        // Act
        var response = await client.GetAsync("/api/brands/current");

        // Assert
        response.EnsureSuccessStatusCode();
        var brand = await response.Content.ReadFromJsonAsync<BrandDto>();
        brand.Should().NotBeNull();
        brand!.Name.Should().Be("Brand 1");
        brand.TenantId.Should().Be("tenant1");
    }
}
