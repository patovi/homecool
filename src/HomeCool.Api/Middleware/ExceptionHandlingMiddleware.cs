using System.Text.Json;

namespace HomeCool.Api.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception for {Method} {Path}", context.Request.Method, context.Request.Path);
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";
            var error = new { error = "Ein interner Fehler ist aufgetreten." };
            await context.Response.WriteAsync(JsonSerializer.Serialize(error));
        }
    }
}
