using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUD.NET.Filters.ResultFilters;

public class PersonAlwaysRunResultFilter : IAlwaysRunResultFilter
{
    public void OnResultExecuted(ResultExecutedContext context)
    {
        //TO DO: before logic here
    }

    public void OnResultExecuting(ResultExecutingContext context)
    {
        if (context.Filters.OfType<SkipFilter>().Any())
        {
            return;
        }
    }
}
