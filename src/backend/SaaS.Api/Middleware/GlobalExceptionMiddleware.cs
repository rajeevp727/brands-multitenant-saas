using System.Net;
using System.Text.Json;
using Serilog;

namespace SaaS.Api.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public GlobalExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var errorId = Guid.NewGuid().ToString();
        Log.Error(exception, "Error ID: {ErrorId} - Unhandled exception occurred", errorId);

        var code = HttpStatusCode.InternalServerError;
        var result = JsonSerializer.Serialize(new 
        { 
            errorId = errorId,
            message = "An internal server error occurred. Please contact support with the Error ID.",
            details = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development" ? exception.Message : null
        });

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)code;

        // Security Headers
        context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
        context.Response.Headers.Append("X-Frame-Options", "DENY");
        context.Response.Headers.Append("Content-Security-Policy", "default-src 'self'; frame-ancestors 'none';");
        context.Response.Headers.Append("Strict-Transport-Security", "max-age=31536000; includeSubDomains");

        if (!context.Response.Headers.ContainsKey("Access-Control-Allow-Origin"))
        {
            context.Response.Headers.Append("Access-Control-Allow-Origin", "*");
        }

        return context.Response.WriteAsync(result);
    }
}
