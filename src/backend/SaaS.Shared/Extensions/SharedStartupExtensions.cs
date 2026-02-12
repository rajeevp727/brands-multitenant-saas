using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Serilog;
using Microsoft.OpenApi.Models;

namespace SaaS.Shared.Extensions;

public static class SharedStartupExtensions
{
    public static Microsoft.AspNetCore.Authentication.AuthenticationBuilder AddSaaSAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtKey = configuration["Jwt:Key"] ?? "super_secret_key_1234567890_antigravity_saas";
        var issuer = configuration["Jwt:Issuer"];
        var audience = configuration["Jwt:Audience"];

        var builder = services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        // Add Cookie Authentication for external OAuth providers
        .AddCookie(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme, options =>
        {
            options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
            options.SlidingExpiration = true;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = !string.IsNullOrEmpty(issuer),
                ValidateAudience = !string.IsNullOrEmpty(audience),
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = issuer,
                ValidAudience = audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
            };
        });

        // Add External Providers if configured
        if (!string.IsNullOrEmpty(configuration["Authentication:Google:ClientId"]))
        {
            builder.AddGoogle(options =>
            {
                options.ClientId = configuration["Authentication:Google:ClientId"]!;
                options.ClientSecret = configuration["Authentication:Google:ClientSecret"]!;
                
                // Explicit callback path (default is /signin-google)
                options.CallbackPath = "/signin-google";
                
                // Use Cookie authentication for external sign-in
                options.SignInScheme = Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme;
                
                // Add error handling and logging
                options.Events = new Microsoft.AspNetCore.Authentication.OAuth.OAuthEvents
                {
                    OnRemoteFailure = context =>
                    {
                        Log.Error("Google OAuth failed: {Error}", context.Failure?.Message ?? "Unknown error");
                        context.Response.Redirect("/login?error=oauth_failed");
                        context.HandleResponse();
                        return Task.CompletedTask;
                    },
                    OnTicketReceived = context =>
                    {
                        Log.Information("Google OAuth ticket received for user: {Email}", 
                            context.Principal?.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value ?? "Unknown");
                        return Task.CompletedTask;
                    }
                };
            });
            
            Log.Information("✅ Google OAuth configured with ClientId: {ClientId}", 
                configuration["Authentication:Google:ClientId"]!.Substring(0, 20) + "...");
        }

        if (!string.IsNullOrEmpty(configuration["Authentication:Facebook:AppId"]))
        {
            builder.AddFacebook(options =>
            {
                options.AppId = configuration["Authentication:Facebook:AppId"]!;
                options.AppSecret = configuration["Authentication:Facebook:AppSecret"]!;
                options.SignInScheme = Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme;
            });
        }

        return builder;
    }

    public static void AddSaaSCors(this IServiceCollection services, IConfiguration configuration, string policyName = "SaaSPolicy")
    {
        var frontendUrl = configuration["FRONTEND_URL"] ?? configuration["FrontendUrl"] ?? configuration["ConnectionStrings:FrontendUrl"];

        services.AddCors(options =>
        {
            options.AddPolicy(policyName, policy =>
            {
                policy.SetIsOriginAllowed(origin => 
                {
                    if (string.IsNullOrEmpty(origin)) return false;

                    // Local development
                    if (origin.StartsWith("http://localhost") || 
                        origin.StartsWith("http://127.0.0.1") ||
                        origin.StartsWith("http://192.168.")) return true;

                    // Production/Staging static domains
                    if (origin.Contains("rajeevstech.in") || 
                        origin.Contains("rajeevstech.com") ||
                        origin.Contains("greenpantry.in") ||
                        origin.Contains("vercel.app")) return true;

                    // Dynamic FRONTEND_URL from environment
                    if (!string.IsNullOrEmpty(frontendUrl) && origin.Equals(frontendUrl.TrimEnd('/'), StringComparison.OrdinalIgnoreCase))
                        return true;

                    return false;
                })
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
            });
        });
    }

    public static void AddSaaSSwagger(this IServiceCollection services, string title, string version = "v1")
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc(version, new OpenApiInfo { Title = title, Version = version });
            
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme.",
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
    }
}
