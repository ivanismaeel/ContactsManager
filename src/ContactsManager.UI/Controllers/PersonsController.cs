using CRUD.NET.Filters;
using CRUD.NET.Filters.ActionFilters;

using CRUD.NET.Filters.ExceptionFilters;
using CRUD.NET.Filters.ResourceFilters;
using CRUD.NET.Filters.ResultFilters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rotativa.AspNetCore;
using Rotativa.AspNetCore.Options;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace CRUD.NET.Controllers
{

    [Route("[controller]")]
    //[TypeFilter(typeof(ResponseHeaderActionFilter), Arguments = new object[] { "My-Key-From-Controller", "My-Value-From-Controller", 3 }, Order = 3)]

    [ResponseHeaderFilterFactory("My-Key-From-Controller", "My-Value-From-Controller", 3)]
    [TypeFilter(typeof(HandleExceptionFilter))]
    [TypeFilter(typeof(PersonAlwaysRunResultFilter))]
    public class PersonsController(
        IPersonsGetterService personsGetterService,
        IPersonsAdderService personsAdderService,
        IPersonsUpdaterService personsUpdaterService,
        IPersonsDeleterService personsDeleterService,
        IPersonsSorterService personsSorterService,
        ICountriesGetterService countriesGetterService,
        ILogger<PersonsController> logger
        ) : Controller
    {
        [Route("[action]")]
        [Route("/")]
        [ServiceFilter(typeof(PersonsListActionFilter), Order = 4)]

        //[TypeFilter(typeof(ResponseHeaderActionFilter), Arguments = new object[] { "MyKey-FromAction", "MyValue-From-Action", 1 }, Order = 1)]

        [ResponseHeaderFilterFactory("MyKey-FromAction", "MyValue-From-Action", 1)]

        [TypeFilter(typeof(PersonsListResultFilter))]
        [SkipFilter]
        public async Task<IActionResult> Index(string searchBy, string? searchString,
       string sortBy = nameof(PersonResponse.PersonName), SortOrderOptions sortOrder = SortOrderOptions.ASC)
        {
            logger.LogInformation("Index action Method of PersonsController called");

            logger.LogDebug("Index action called with searchBy: {searchBy}, searchString: {searchString}, sortBy: {sortBy}, sortOrder: {sortOrder}",
                searchBy, searchString, sortBy, sortOrder);

            var persons = await personsGetterService.GetFilteredPersons(searchBy, searchString);

            var sortedPersons = await personsSorterService.GetSortedPersons(persons, sortBy, sortOrder);

            return View(sortedPersons);
        }

        [Route("[action]")]
        [HttpGet]
        [ResponseHeaderFilterFactory("my-key", "my-value", 4)]
        public async Task<IActionResult> Create()
        {
            var countries = await countriesGetterService.GetAllCountries();
            ViewBag.Countries = countries.Select(temp =>
                new SelectListItem()
                {
                    Text = temp.CountryName,
                    Value = temp.CountryID.ToString()
                });

            return View();
        }

        [HttpPost]
        //Url: persons/create
        [Route("[action]")]
        [TypeFilter(typeof(PersonCreateAndEditPostActionFilter))]
        [TypeFilter(typeof(FeatureDisabledResourceFilter), Arguments = new object[] { false })]
        public async Task<IActionResult> Create(PersonAddRequest personRequest)
        {
            var personResponse = await personsAdderService.AddPerson(personRequest);
            return RedirectToAction("Index", "Persons");
        }

        [HttpGet]
        [Route("[action]/{personID}")] //Eg: /persons/edit/1
                                       //[TypeFilter(typeof(TokenResultFilter))]
        public async Task<IActionResult> Edit(Guid PersonID)
        {
            var personResponse = await personsGetterService.GetPersonByPersonID(PersonID);
            if (personResponse == null)
            {
                return RedirectToAction("Index", "Persons");
            }

            var personUpdateRequest = personResponse.ToPersonUpdateRequest();

            var countries = await countriesGetterService.GetAllCountries();

            ViewBag.Countries = countries.Select(temp =>
                new SelectListItem() { Text = temp.CountryName, Value = temp.CountryID.ToString() }
            );

            return View(personUpdateRequest);
        }

        [HttpPost]
        [Route("[action]/{personID}")]
        [TypeFilter(typeof(PersonCreateAndEditPostActionFilter))]
        // [TypeFilter(typeof(TokenAuthorizationFilter))]

        public async Task<IActionResult> Edit(PersonUpdateRequest personRequest)
        {
            var personResponse = await personsGetterService.GetPersonByPersonID(personRequest.PersonID);
            if (personResponse == null)
            {
                return RedirectToAction("Index", "Persons");
            }

            var updatePerson = await personsUpdaterService.UpdatePerson(personRequest);
            return RedirectToAction("Index", "Persons");

        }

        [Route("[action]/{PersonID}")]
        [HttpGet]
        public async Task<IActionResult> Delete(Guid? PersonID)
        {
            var personResponse = await personsGetterService.GetPersonByPersonID(PersonID);
            if (personResponse == null)
            {
                return RedirectToAction("Index", "Persons");
            }
            return View(personResponse);
        }

        [Route("[action]/{PersonID}")]
        [HttpPost]
        public async Task<IActionResult> Delete(PersonUpdateRequest personUpdateRequest)
        {
            var personResponse = await personsGetterService.GetPersonByPersonID(personUpdateRequest.PersonID);
            if (personResponse == null)
            {
                return RedirectToAction("Index", "Persons");
            }

            await personsDeleterService.DeletePerson(personUpdateRequest.PersonID);
            return RedirectToAction("Index", "Persons");
        }

        [Route("PersonsPDF")]
        public async Task<IActionResult> PersonsPDF()
        {
            var persons = await personsGetterService.GetAllPersons();

            return new ViewAsPdf("PersonsPDF", persons, ViewData)
            {
                PageMargins = new Margins
                {
                    Left = 20,
                    Right = 20,
                    Top = 20,
                    Bottom = 20
                },
                PageOrientation = Orientation.Landscape
            };
        }

        [Route("PersonsCSV")]
        public async Task<IActionResult> PersonsCSV()
        {
            var memoryStream = await personsGetterService.GetPersonsCSV();

            return File(memoryStream, "application/octet-stream", "Persons.csv");
        }

        [Route("PersonsExcel")]
        public async Task<IActionResult> PersonsExcel()
        {
            var memoryStream = await personsGetterService.GetPersonsExcel();

            return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Persons.xlsx");
        }
    }
}
