using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SaaS.Infrastructure.Persistence;
using Xunit;

namespace SaaS.Api.Tests.Integration;

public class StartupTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public StartupTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Remove existing DbContext registration
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Add InMemory DbContext
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryDbForStartupTest");
                });
            });
        });
    }

    [Fact]
    public void App_Starts_Successfully()
    {
        var client = _factory.CreateClient();
        Assert.NotNull(client);
    }

    [Fact]
    public async Task HealthEndpoint_ReturnsOk()
    {
         var client = _factory.CreateClient();
         var response = await client.GetAsync("/health");
         response.EnsureSuccessStatusCode();
         var content = await response.Content.ReadAsStringAsync();
         Assert.Equal("\"Healthy\"", content);
    }
}
