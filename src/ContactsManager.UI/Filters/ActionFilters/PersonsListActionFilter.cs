using CRUD.NET.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace CRUD.NET.Filters.ActionFilters;

public class PersonsListActionFilter(ILogger<PersonsListActionFilter> logger) : IActionFilter
{
    public void OnActionExecuted(ActionExecutedContext context)
    {
        // Logic after the action executes
        logger.LogInformation("{FilterName}.{MethodName} method",
            nameof(PersonsListActionFilter), nameof(OnActionExecuted));

        PersonsController personsController = (PersonsController)context.Controller;

        IDictionary<string, object?>? parameters = (IDictionary<string, object?>?)context.HttpContext.Items["arguments"];

        if (parameters != null)
        {
            if (parameters.ContainsKey("searchBy"))
            {
                personsController.ViewData["CurrentSearchBy"] = Convert.ToString(parameters["searchBy"]);
            }

            if (parameters.ContainsKey("searchString"))
            {
                personsController.ViewData["CurrentSearchString"] = Convert.ToString(parameters["searchString"]);
            }

            if (parameters.ContainsKey("sortBy"))
            {
                personsController.ViewData["CurrentSortBy"] = Convert.ToString(parameters["sortBy"]);
            }
            else
            {
                personsController.ViewData["CurrentSortBy"] = nameof(PersonResponse.PersonName);
            }

            if (parameters.ContainsKey("sortOrder"))
            {
                personsController.ViewData["CurrentSortOrder"] = Convert.ToString(parameters["sortOrder"]);
            }
            else
            {
                personsController.ViewData["CurrentSortOrder"] = SortOrderOptions.ASC.ToString();
            }
        }

        personsController.ViewBag.SearchFields = new Dictionary<string, string>()
      {
        { nameof(PersonResponse.PersonName), "Person Name" },
        { nameof(PersonResponse.Email), "Email" },
        { nameof(PersonResponse.DateOfBirth), "Date of Birth" },
        { nameof(PersonResponse.Gender), "Gender" },
        { nameof(PersonResponse.CountryID), "Country" },
        { nameof(PersonResponse.Address), "Address" }
      };
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        context.HttpContext.Items["arguments"] = context.ActionArguments;

        // Logic before the action executes
        logger.LogInformation("{FilterName}.{MethodName} method",
            nameof(PersonsListActionFilter), nameof(OnActionExecuting));

        if (context.ActionArguments.TryGetValue("searchBy", out object? value))
        {
            var searchBy = Convert.ToString(value);
            if (!string.IsNullOrEmpty(searchBy))
            {
                var searchByOptions = new List<string>
                {
                    nameof(PersonResponse.PersonName),
                    nameof(PersonResponse.Email),
                    nameof(PersonResponse.DateOfBirth),
                    nameof(PersonResponse.Gender),
                    nameof(PersonResponse.CountryID),
                    nameof(PersonResponse.Address)
                };

                //reset the searchBy paramer value
                if (searchByOptions.Any(temp => temp == searchBy) == false)
                {
                    logger.LogInformation("searchBy actual value {searchBy}", searchBy);
                    context.ActionArguments["searchBy"] = nameof(PersonResponse.PersonName);
                    logger.LogInformation("searchBy updated value {searchBy}", context.ActionArguments["searchBy"]);
                }
            }
        }
    }
}

