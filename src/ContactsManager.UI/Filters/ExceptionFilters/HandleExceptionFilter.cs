using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUD.NET.Filters.ExceptionFilters;

public class HandleExceptionFilter(ILogger<HandleExceptionFilter> logger,
    IHostEnvironment hostEnvironment) : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        logger.LogError("{FilterName}.{MethodName}\n{ExceptionType}\n{ExceptionMessage}",
            nameof(HandleExceptionFilter), nameof(OnException), context.Exception.GetType().ToString()
            , context.Exception.Message);


        if (hostEnvironment.IsDevelopment())
        {
            context.Result = new ContentResult()
            {
                Content = context.Exception.Message,
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }
    }
}
