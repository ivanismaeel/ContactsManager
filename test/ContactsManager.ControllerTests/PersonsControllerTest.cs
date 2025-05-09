using AutoFixture;
using CRUD.NET.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace CRUDTests;

public class PersonsControllerTest
{
    private readonly IPersonsGetterService _personsGetterService;
    private readonly IPersonsAdderService _personAdderService;
    private readonly IPersonsUpdaterService _personUpdaterService;
    private readonly IPersonsDeleterService _personDeleterService;
    private readonly IPersonsSorterService _personSorterService;
    private readonly ICountriesGetterService _countriesGetterService;
    private readonly ILogger<PersonsController> _logger;

    private readonly Mock<IPersonsGetterService> _personsGetterServiceMock;
    private readonly Mock<IPersonsAdderService> _personsAdderServiceMock;
    private readonly Mock<IPersonsUpdaterService> _personsUpdaterServiceMock;
    private readonly Mock<IPersonsDeleterService> _personsDeleterServiceMock;
    private readonly Mock<IPersonsSorterService> _personsSorterServiceMock;
    private readonly Mock<ICountriesGetterService> _countriesGetterServiceMock;
    private readonly Mock<ILogger<PersonsController>> _loggerMock;

    private readonly IFixture _fixture;
    public PersonsControllerTest()
    {
        _fixture = new Fixture();

        _personsGetterServiceMock = new Mock<IPersonsGetterService>();
        _personsAdderServiceMock = new Mock<IPersonsAdderService>();
        _personsUpdaterServiceMock = new Mock<IPersonsUpdaterService>();
        _personsDeleterServiceMock = new Mock<IPersonsDeleterService>();
        _personsSorterServiceMock = new Mock<IPersonsSorterService>();
        _countriesGetterServiceMock = new Mock<ICountriesGetterService>();
        _loggerMock = new Mock<ILogger<PersonsController>>();

        _personsGetterService = _personsGetterServiceMock.Object;
        _personAdderService = _personsAdderServiceMock.Object;
        _personUpdaterService = _personsUpdaterServiceMock.Object;
        _personDeleterService = _personsDeleterServiceMock.Object;
        _personSorterService = _personsSorterServiceMock.Object;
        _countriesGetterService = _countriesGetterServiceMock.Object;
        _logger = _loggerMock.Object;
    }

    [Fact]
    public async Task Index_shouldReturnIndexViewWithPersonsList()
    {
        // Arrange
        var personResponsesList = _fixture.Create<List<PersonResponse>>();

        var controller = new PersonsController(
                _personsGetterService,
                _personAdderService,
                _personUpdaterService,
                _personDeleterService,
                _personSorterService,
                _countriesGetterService,
                _logger
        );

        _personsGetterServiceMock.Setup(x => x.GetFilteredPersons(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(personResponsesList);

        _personsSorterServiceMock.Setup(x => x.GetSortedPersons(
                It.IsAny<List<PersonResponse>>(),
                It.IsAny<string>(),
                It.IsAny<SortOrderOptions>()))
            .ReturnsAsync(personResponsesList);

        // Act
        var result = await controller.Index(
            _fixture.Create<string>(),
            _fixture.Create<string>(),
            _fixture.Create<string>(),
            _fixture.Create<SortOrderOptions>());

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        viewResult.ViewData.Model.Should().BeAssignableTo<IEnumerable<PersonResponse>>();
        viewResult.ViewData.Model.Should().Be(personResponsesList);
    }

    [Fact]
    public async Task Create_IfNoModelError_ToReturnRedirectToIndex()
    {
        // Arrange
        var personsController = new PersonsController(
                _personsGetterService,
                _personAdderService,
                _personUpdaterService,
                _personDeleterService,
                _personSorterService,
                _countriesGetterService,
                _logger);

        var personAddRequest = _fixture.Create<PersonAddRequest>();
        var personResponse = _fixture.Create<PersonResponse>();

        var countriesList = _fixture.Create<List<CountryResponse>>();

        _countriesGetterServiceMock.Setup(x => x.GetAllCountries())
            .ReturnsAsync(countriesList);

        _personsAdderServiceMock.Setup(x => x.AddPerson(It.IsAny<PersonAddRequest>()))
            .ReturnsAsync(personResponse);

        IActionResult result = await personsController.Create(personAddRequest);

        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);

        redirectToActionResult.ActionName.Should().Be("Index");
    }
}