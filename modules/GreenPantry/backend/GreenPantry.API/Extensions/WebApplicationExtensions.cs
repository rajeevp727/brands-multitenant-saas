using GreenPantry.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace GreenPantry.API.Extensions;

public static class WebApplicationExtensions
{
    public static async Task InitializeDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        try
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await DbInitializer.InitializeAsync(dbContext);
            Log.Information("SQL Server database initialized and seeded successfully");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occurred while initializing the database.");
            throw; // Re-throw to ensure startup fails if DB can't be initialized
        }
    }

    public static void UseSwaggerWithRedirect(this WebApplication app)
    {
        // Enable Swagger in all environments for easier debugging
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "GreenPantry API V1");
        });

        // Redirect root URL to swagger
        app.MapGet("/", () => Results.Redirect("/swagger/index.html"));
    }

    public static void MapHealthCheckEndpoints(this WebApplication app)
    {
        app.MapGet("/health", () => new 
        { 
            Status = "Healthy", 
            Timestamp = DateTime.UtcNow,
            Service = "GreenPantry API"
        });

        app.MapGet("/health/database", async (AppDbContext dbContext) =>
        {
            try
            {
                var canConnect = await dbContext.Database.CanConnectAsync();
                return Results.Ok(new 
                { 
                    Status = canConnect ? "Healthy" : "Unhealthy", 
                    DatabaseConnected = canConnect, 
                    Timestamp = DateTime.UtcNow 
                });
            }
            catch (Exception ex)
            {
                return Results.Json(new 
                { 
                    Status = "Unhealthy", 
                    Error = ex.Message, 
                    Timestamp = DateTime.UtcNow 
                }, statusCode: 500);
            }
        });
    }
}
