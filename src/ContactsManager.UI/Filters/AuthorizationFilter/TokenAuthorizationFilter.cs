using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUD.NET.Filters.AuthorizationFilter;

public class TokenAuthorizationFilter : IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (context.HttpContext.Request.Cookies.ContainsKey("Auth-key") == false)
        {
            context.Result = new StatusCodeResult(StatusCodes.Status401Unauthorized);
            return;
        }

        if (context.HttpContext.Request.Cookies["Auth-key"] != "A100")
        {
            context.Result = new StatusCodeResult(StatusCodes.Status401Unauthorized);
            return;
        }
    }
}
