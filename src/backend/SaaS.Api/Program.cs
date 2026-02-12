using SaaS.Application;
using SaaS.Infrastructure;
using SaaS.Application.Interfaces;
using SaaS.Infrastructure.Services;
using SaaS.Api.Services;
using SaaS.Api.Middleware;
using SaaS.Shared.Extensions;
using SaaS.Domain.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Serilog;
using Serilog.Events;
using Serilog.Filters;
using Serilog.Expressions;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using SaaS.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;
using Serilog.AspNetCore;
using Microsoft.EntityFrameworkCore;

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
                               Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto;
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
