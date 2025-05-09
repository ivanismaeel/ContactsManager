namespace CRUD.NET.Middleware;

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
            if (ex.InnerException != null)
            {
                logger.LogError("{ExceptionType}{ExceptionMessage}",
                    ex.InnerException.GetType().ToString(), ex.InnerException.Message);
            }
            else
            {
                logger.LogError("{ExceptionType}{ExceptionMessage}",
                    ex.GetType().ToString(), ex.Message);
            }

            // context.Response.StatusCode = 500;
            // await context.Response.WriteAsync("An unexpected fault happened. Try again later.");

            throw;
        }
    }
}

// Create an extension method to register the middleware
public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionHandlingMiddleware(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}