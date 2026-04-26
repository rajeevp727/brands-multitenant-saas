using System.Net;
using System.Text.Json;

namespace GreenPantry.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        
        var (statusCode, message) = exception switch
        {
            UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "Unauthorized access"),
            KeyNotFoundException => (HttpStatusCode.NotFound, "The requested resource was not found"),
            InvalidOperationException => (HttpStatusCode.BadRequest, exception.Message),
            _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred while processing your request")
        };

        context.Response.StatusCode = (int)statusCode;

        var response = new
        {
            Status = (int)statusCode,
            Message = message,
            Details = context.HostingEnvironment().IsDevelopment() ? exception.Message : null,
            TraceId = context.TraceIdentifier
        };

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        await context.Response.WriteAsJsonAsync(response, options);
    }
}

public static class HttpContextExtensions
{
    public static IWebHostEnvironment HostingEnvironment(this HttpContext context)
        => context.RequestServices.GetRequiredService<IWebHostEnvironment>();
}
