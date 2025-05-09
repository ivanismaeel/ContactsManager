using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUD.NET.Filters.ActionFilters;

public class ResponseHeaderFilterFactoryAttribute(string? key, string? value, int order) : Attribute, IFilterFactory
{
    public bool IsReusable => false;
    private int Order { get; set; } = order;

    //Controller -> FilterFactory -> Filter
    public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
    {

        if (key == null || value == null)
        {
            throw new ArgumentNullException(nameof(key), "Key and Value must be set");
        }

        var filter = serviceProvider.GetRequiredService<ResponseHeaderActionFilter>();
        filter.Key = key;
        filter.Value = value;
        filter.Order = Order;

        //return filter object
        return filter;
    }
}

public class ResponseHeaderActionFilter(ILogger<ResponseHeaderActionFilter> logger) : IAsyncActionFilter, IOrderedFilter
{
    public string Key { get; set; } = null!;
    public string Value { get; set; } = null!;
    public int Order { get; set; }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        logger.LogInformation("Before logic - ResponseHeaderActionFilter");

        await next(); //calls the subsequent filter or action method

        logger.LogInformation("Before logic - ResponseHeaderActionFilter");

        context.HttpContext.Response.Headers[Key] = Value;
    }
}
