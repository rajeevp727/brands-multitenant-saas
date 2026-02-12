using GreenPantry.API.Extensions;
using GreenPantry.API.Middleware;
using GreenPantry.Application.Extensions;
using GreenPantry.Infrastructure.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "GreenPantry API", Version = "v1" });
    c.CustomSchemaIds(type => type.FullName); 
});

// Add Layer Services
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

// Add Auth & CORS
GreenPantry.API.Extensions.ServiceCollectionExtensions.AddJwtAuthentication(builder.Services, builder.Configuration);
builder.Services.AddCors(options =>
{
    var frontendUrl = builder.Configuration["FRONTEND_URL"] ?? builder.Configuration["FrontendUrl"] ?? "https://rajeevs-pvt-ltd.vercel.app";
    
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.SetIsOriginAllowed(origin => 
        {
            if (string.IsNullOrEmpty(origin)) return false;

            // Local development
            if (origin.StartsWith("http://localhost") || 
                origin.StartsWith("http://127.0.0.1") ||
                origin.StartsWith("http://192.168.")) return true;

            // Production domains
            if (origin.Contains("greenpantry.in") || origin.Contains("vercel.app")) return true;

            // Dynamic FRONTEND_URL
            if (!string.IsNullOrEmpty(frontendUrl) && origin.Equals(frontendUrl.TrimEnd('/'), StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        })
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseSwaggerWithRedirect();

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

// Map Endpoints
app.MapControllers();
app.MapHealthCheckEndpoints();

// Initialize Database
await app.InitializeDatabaseAsync();

app.Run();
