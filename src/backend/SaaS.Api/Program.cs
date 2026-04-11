using Microsoft.AspNetCore.RateLimiting;
using SaaS.Api.Core.Tenancy;
using SaaS.Api.Middleware;
using SaaS.Api.Services;
using SaaS.Application;
using SaaS.Application.Interfaces;
using SaaS.Domain.Interfaces;
using SaaS.Infrastructure;
using SaaS.Infrastructure.Repositories;
using SaaS.Infrastructure.Services;
using SaaS.Shared.Extensions;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Serilog.Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    // Global log file
    .WriteTo.File("Logs/all-brands/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddHttpClient();
builder.Services.AddScoped<ISystemService, SystemService>();
builder.Services.AddHostedService<KeepAliveService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Shared Extensions
builder.Services.AddSaaSSwagger("SaaS API");
builder.Services.AddSaaSAuthentication(builder.Configuration);
builder.Services.AddSaaSCors(builder.Configuration, "TenantPolicy");

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ITenantProvider, HttpContextTenantProvider>();
builder.Services.AddScoped<TenantContext>();
builder.Services.AddMemoryCache();
builder.Services.AddScoped<ITenantService, TenantService>();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddFixedWindowLimiter("fixed", opt =>
    {
        opt.Window = TimeSpan.FromSeconds(10);
        opt.PermitLimit = 100;
        opt.QueueLimit = 2;
        opt.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
    });
});


// Configure Forwarded Headers for Render/Proxies
builder.Services.Configure<Microsoft.AspNetCore.Builder.ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor | 
                               Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto |
                               Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedHost;
    // CRITICAL for Render/Vercel proxies: Clear restrictions
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

// Fix: Force Secure Cookies for Local/Proxy
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.Secure = builder.Environment.IsDevelopment() ? CookieSecurePolicy.SameAsRequest : CookieSecurePolicy.Always;
    options.MinimumSameSitePolicy = builder.Environment.IsDevelopment() ? SameSiteMode.Lax : SameSiteMode.None;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // DEBUG: Dump Users table columns to identify where "Password" column came from
    try {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<SaaS.Infrastructure.Persistence.ApplicationDbContext>();
        // Using ADO.NET because SqlQueryRaw is for entities
        using var conn = context.Database.GetDbConnection();
        await conn.OpenAsync();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT column_name FROM information_schema.columns WHERE table_name = 'Users' AND table_schema = 'public'";
        var columns = new List<string>();
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync()) columns.Add(reader.GetString(0));
        Console.WriteLine("DEBUG SCHEMA [public.Users]: " + string.Join(", ", columns));
    } catch (Exception ex) { Console.WriteLine("DEBUG SCHEMA ERROR: " + ex.Message); }

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging(options =>
{
    options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms (Tenant: {TenantId})";
    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
    {
        diagnosticContext.Set("TenantId", httpContext.Request.Headers["X-Tenant-Id"].FirstOrDefault());
    };
});

app.UseForwardedHeaders();

app.UseMiddleware<GlobalExceptionMiddleware>();

// Critical: UseCookiePolicy must be before Authentication
app.UseCookiePolicy();

app.UseCors("TenantPolicy");

// Seed Database (Safe invocation)
using (var scope = app.Services.CreateScope())
{
    try 
    {
        await SaaS.Infrastructure.Persistence.DbInitializer.SeedAsync(scope.ServiceProvider);
    }
    catch (Exception ex)
    {
        Serilog.Log.Error(ex, "An error occurred during database seeding.");
    }
}

app.UseMiddleware<TenantMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.UseRateLimiter();

app.MapGet("/health", () => Results.Ok("Healthy"));

app.MapControllers();

app.MapGet("/api/debug/schema", async (SaaS.Infrastructure.Persistence.ApplicationDbContext ctx) => {
    var dtos = new System.Collections.Generic.List<string>();
    using var command = ctx.Database.GetDbConnection().CreateCommand();
    command.CommandText = "SELECT column_name FROM information_schema.columns WHERE table_name = 'Users'";
    await ctx.Database.OpenConnectionAsync();
    using var reader = await command.ExecuteReaderAsync();
    while (await reader.ReadAsync()) { dtos.Add(reader.GetString(0)); }
    return Microsoft.AspNetCore.Http.Results.Ok(dtos);
});

app.MapGet("/api/debug/force-schema", async (SaaS.Infrastructure.Persistence.ApplicationDbContext ctx) => {
    try {
        await Microsoft.EntityFrameworkCore.RelationalDatabaseFacadeExtensions.ExecuteSqlRawAsync(ctx.Database, "ALTER TABLE \"Users\" ADD COLUMN \"CreatedAt\" timestamp with time zone NOT NULL DEFAULT NOW();");
        await Microsoft.EntityFrameworkCore.RelationalDatabaseFacadeExtensions.ExecuteSqlRawAsync(ctx.Database, "ALTER TABLE \"Users\" ADD COLUMN \"RefreshToken\" text;");
        return Microsoft.AspNetCore.Http.Results.Ok("Forced");
    } catch (System.Exception e) { return Microsoft.AspNetCore.Http.Results.BadRequest(e.ToString()); }
});

try
{
    Serilog.Log.Information("Starting Multi-Tenant SaaS API...");
    app.Run();
}
catch (Exception ex)
{
    Serilog.Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Serilog.Log.CloseAndFlush();
}

public partial class Program { }
