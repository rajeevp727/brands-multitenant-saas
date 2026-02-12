using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Vendor.API.Middleware;
using Vendor.Application.Interfaces;
using Vendor.Application.Mapping;
using Vendor.Application.Validators;
using Vendor.Infrastructure.Data;
using Vendor.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Seed Database
if (args.Length > 0 && args[0] == "seed")
{
    var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
    optionsBuilder.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
    using var context = new ApplicationDbContext(optionsBuilder.Options);
    await Vendor.Infrastructure.Data.DbInitializer.InitializeAsync(context);
    return;
}

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Vendor API",
        Version = "v1",
        Description = "Vendor Management API for BangaruKottu"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] 
                ?? throw new InvalidOperationException("JWT Key not configured")))
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("VendorOnly", policy => policy.RequireRole("Vendor"));
});

builder.Services.AddCors(options =>
{
    var frontendUrl = builder.Configuration["FRONTEND_URL"] ?? builder.Configuration["FrontendUrl"] ?? "https://rajeevs-pvt-ltd.vercel.app";
    
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.SetIsOriginAllowed(origin => 
        {
            if (string.IsNullOrEmpty(origin)) return false;

            // Local development
            if (origin.StartsWith("http://localhost") || 
                origin.StartsWith("http://127.0.0.1") ||
                origin.StartsWith("http://192.168.")) return true;

            // Production domains
            if (origin.Contains("bangarukottu.in") || origin.Contains("vercel.app") || origin.Contains("rajeevstech.in")) return true;

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

builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IVendorRepository, VendorRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();

// builder.Services.AddScoped<IAuthService, Vendor.Infrastructure.Services.AuthService>();
builder.Services.AddScoped<IVendorService, Vendor.Application.Services.VendorService>();
builder.Services.AddScoped<IProductService, Vendor.Application.Services.ProductService>();
builder.Services.AddScoped<IOrderService, Vendor.Application.Services.OrderService>();
builder.Services.AddScoped<ICategoryService, Vendor.Application.Services.CategoryService>();
builder.Services.AddScoped<IDashboardService, Vendor.Application.Services.DashboardService>();
builder.Services.AddScoped<IAuthService, Vendor.Infrastructure.Services.AuthService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Vendor API v1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors("AllowReactApp");

app.UseAuthentication();
app.UseAuthorization();

// Auto-seed in Development
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await Vendor.Infrastructure.Data.DbInitializer.InitializeAsync(context);
}

app.MapControllers();

app.Run();

