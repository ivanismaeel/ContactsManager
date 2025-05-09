using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUD.NET.Filters.ResultFilters;

public class PersonsListResultFilter(ILogger<PersonsListResultFilter> logger) : IAsyncResultFilter
{
    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        //TO DO: before logic
        logger.LogInformation("{FilterName}.{MethodName} - before", nameof(PersonsListResultFilter), nameof(OnResultExecutionAsync));

        context.HttpContext.Response.Headers["Last-Modified"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm");

        await next(); //call the subsequent filter [or] IActionResult

        //TO DO: after logic
        logger.LogInformation("{FilterName}.{MethodName} - after", nameof(PersonsListResultFilter), nameof(OnResultExecutionAsync));
    }
}
