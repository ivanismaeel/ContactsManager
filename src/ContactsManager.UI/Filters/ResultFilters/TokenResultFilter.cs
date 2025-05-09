using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUD.NET.Filters.ResultFilters;

public class TokenResultFilter : IResultFilter
{
    public void OnResultExecuted(ResultExecutedContext context)
    {
        throw new NotImplementedException();
    }

    public void OnResultExecuting(ResultExecutingContext context)
    {
        context.HttpContext.Response.Cookies.Append("Auth-key", "A100");
    }
}
