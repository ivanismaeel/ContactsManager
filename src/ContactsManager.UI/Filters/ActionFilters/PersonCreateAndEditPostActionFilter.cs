using CRUD.NET.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServiceContracts;

namespace CRUD.NET.Filters;

public class PersonCreateAndEditPostActionFilter(ILogger<PersonCreateAndEditPostActionFilter> logger, ICountriesGetterService countriesGetterService) : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context.Controller is PersonsController personsController)
        {
            if (!personsController.ModelState.IsValid)
            {
                var countries = await countriesGetterService.GetAllCountries();
                personsController.ViewBag.Countries = countries.Select(temp =>
                new SelectListItem()
                {
                    Text = temp.CountryName,
                    Value = temp.CountryID.ToString()
                });

                personsController.ViewBag.Errors = personsController.ModelState.Values.SelectMany(x => x.Errors)
                    .Select(x => x.ErrorMessage)
                    .ToList();

                var personRequest = context.ActionArguments["personRequest"];
                context.Result = personsController.View(personRequest);

                return;
            }
            else
            {
                await next();
            }
        }
        else
        {
            await next();

            logger.LogInformation("PersonCreateAndEditPostActionFilter after next(): {FilterName}.{MethodName} method",
                nameof(PersonCreateAndEditPostActionFilter), nameof(OnActionExecutionAsync));
        }
    }
}
