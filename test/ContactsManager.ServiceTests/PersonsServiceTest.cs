using System.Linq.Expressions;
using AutoFixture;
using Entities;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using RepositoryContracts;
using Serilog;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;
using Xunit.Abstractions;

namespace CRUDTests;

public class PersonsServiceTest
{
    private readonly IPersonsGetterService _personGetterService;
    private readonly IPersonsAdderService _personAdderService;
    private readonly IPersonsUpdaterService _personUpdaterService;
    private readonly IPersonsDeleterService _personDeleterService;
    private readonly IPersonsSorterService _personSorterService;
    private readonly Mock<IPersonsRepository> _personRepositoryMock;
    private readonly IPersonsRepository _personRepository;
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly IFixture _fixture;

    public PersonsServiceTest(ITestOutputHelper testOutputHelper)
    {
        _fixture = new Fixture();
        _personRepositoryMock = new Mock<IPersonsRepository>();
        _personRepository = _personRepositoryMock.Object;

        var diagnosticContext = new Mock<IDiagnosticContext>();
        var loggerGetterMok = new Mock<ILogger<PersonsGetterService>>();
        var loggerSorterMok = new Mock<ILogger<PersonsSorterService>>();

        _personGetterService = new PersonsGetterService(_personRepository, loggerGetterMok.Object, diagnosticContext.Object);
        _personAdderService = new PersonsAdderService(_personRepository);
        _personUpdaterService = new PersonsUpdaterService(_personRepository);
        _personDeleterService = new PersonsDeleterService(_personRepository);
        _personSorterService = new PersonsSorterService(loggerSorterMok.Object);

        _testOutputHelper = testOutputHelper;
    }

    // Helper Method to Create Person
    private List<Person> CreatePerson(params string[] emails)
    {
        return emails.Select(email =>
            _fixture.Build<Person>()
                .With(temp => temp.Email, email) // Use the actual email parameter
                .With(temp => temp.Country, null as Country) // Set Country to null
                .Create()
        ).ToList();
    }

    // Helper Method to Print Responses
    private void PrintResponses(IEnumerable<object> responses, string header)
    {
        _testOutputHelper.WriteLine(header);
        foreach (var response in responses)
        {
            _testOutputHelper.WriteLine(response.ToString());
        }
    }

    [Fact]
    public async Task AddPerson_NullPerson_ToBeArgumentNullException()
    {
        PersonAddRequest? personAddRequest = null;

        var action = async () =>
        {
            await _personAdderService.AddPerson(personAddRequest);
        };

        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task AddPerson_PersonNameIsNull_ToBeArgumentException()
    {
        var personAddRequest = _fixture.Build<PersonAddRequest>()
            .With(temp => temp.PersonName, null as string)
            .Create();

        var person = personAddRequest.ToPerson();

        _personRepositoryMock.Setup(temp =>
            temp.AddPerson(It.IsAny<Person>()))
            .ReturnsAsync(person);

        var action = async () =>
        {
            await _personAdderService.AddPerson(personAddRequest);
        };

        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task AddPerson_FullPersonDetails_ToBeSuccessful()
    {
        var personAddRequest = _fixture.Build<PersonAddRequest>()
            .With(temp => temp.Email, "person@test.com")
            .Create();

        var person = personAddRequest.ToPerson();
        var personResponseExpected = person.ToPersonResponse();

        _personRepositoryMock.Setup(temp =>
            temp.AddPerson(It.IsAny<Person>()))
            .ReturnsAsync(person);

        var personResponseFromAdd = await _personAdderService.AddPerson(personAddRequest);
        personResponseExpected.PersonID = personResponseFromAdd.PersonID;

        // Assert.True(personResponseFromAdd.PersonID != Guid.Empty);
        personResponseFromAdd.PersonID.Should().NotBe(Guid.Empty);
        personResponseFromAdd.Should().Be(personResponseExpected);
    }

    [Fact]
    public async Task GetPersonByPersonID_NullPersonID_ToBeNull()
    {
        Guid? PersonID = null;

        var personResponseFromGet = await _personGetterService.GetPersonByPersonID(PersonID);

        personResponseFromGet.Should().BeNull();
    }

    [Fact]
    public async Task GetPersonByPersonID_WithPersonID_ToBeSuccessful()
    {
        var person = CreatePerson("person@test.com");

        var personResponseExpected = person.First().ToPersonResponse();

        _personRepositoryMock.Setup(temp =>
            temp.GetPersonByPersonID(It.IsAny<Guid>()))
            .ReturnsAsync(person.First());

        var personResponseFromGet = await _personGetterService.GetPersonByPersonID(person.First().PersonID);

        // Assert.Equal(personResponseFromAdd, personResponseFromGet);
        personResponseFromGet.Should().Be(personResponseExpected);
    }

    [Fact]
    public async Task GetAllPersons_EmptyList()
    {
        _personRepositoryMock.Setup(temp =>
            temp.GetAllPersons())
            .ReturnsAsync([]);

        var personsFromGet = await _personGetterService.GetAllPersons();

        personsFromGet.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllPersons_WithFewPersons_ToBeSuccessful()
    {
        // Create 3 person requests
        var persons = CreatePerson("person_1@test.com", "person_2@test.com", "person_3@test.com");
        var personResponseListExpected = persons.Select(temp => temp.ToPersonResponse()).ToList();

        _personRepositoryMock.Setup(temp => temp.GetAllPersons()).ReturnsAsync(persons);

        var personsListFromGet = await _personGetterService.GetAllPersons();

        // Print and assert responses
        PrintResponses(personResponseListExpected, "Expected:");
        PrintResponses(personsListFromGet, "Actual:");

        personsListFromGet.Should().BeEquivalentTo(personResponseListExpected);
    }

    [Fact]
    public async Task GetFilteredPersons_EmptySearchText_ToBeSuccessful()
    {
        var persons = CreatePerson("person_1@test.com", "person_2@test.com", "person_3@test.com");
        var personResponseListExpected = persons.Select(temp => temp.ToPersonResponse()).ToList();

        _personRepositoryMock.Setup(temp => temp
            .GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>()))
            .ReturnsAsync(persons);

        // Filter persons by name empty
        var personsListFromSearch = await _personGetterService.GetFilteredPersons(nameof(Person.PersonName), "");

        // Print and assert responses
        PrintResponses(personResponseListExpected, "Expected:");
        PrintResponses(personsListFromSearch, "Actual:");

        personsListFromSearch.Should().BeEquivalentTo(personResponseListExpected);
    }

    [Fact]
    public async Task GetFilteredPersons_SearchByPersonName_ToBeSuccessful()
    {
        var persons = CreatePerson("person_1@test.com", "person_2@test.com", "person_3@test.com");
        var personResponseListExpected = persons.Select(temp => temp.ToPersonResponse()).ToList();

        _personRepositoryMock.Setup(temp => temp.GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>()))
            .ReturnsAsync(persons);

        // Filter persons by name containing "te"
        var personsListFromSearch = await _personGetterService.GetFilteredPersons(nameof(Person.PersonName), "te");

        // Print and assert responses
        PrintResponses(personResponseListExpected, "Expected:");
        PrintResponses(personsListFromSearch, "Actual:");

        personsListFromSearch.Should().BeEquivalentTo(personResponseListExpected);
    }

    [Fact]
    public async Task GetSortedPersons_ToBeSuccessful()
    {
        var persons = CreatePerson("person_1@test.com", "person_2@test.com", "person_3@test.com");
        var personResponseListExpected = persons.Select(temp => temp.ToPersonResponse()).ToList();

        _personRepositoryMock.Setup(temp => temp.GetAllPersons()).ReturnsAsync(persons);

        var allPersons = await _personGetterService.GetAllPersons();
        var personsListFromSort = await _personSorterService.GetSortedPersons(allPersons, nameof(Person.PersonName), SortOrderOptions.DESC);

        // Print responses
        PrintResponses(personResponseListExpected.OrderByDescending(p => p.PersonName), "Expected:");
        PrintResponses(personsListFromSort, "Actual:");

        personsListFromSort.Should().BeInDescendingOrder(p => p.PersonName);
    }

    [Fact]
    public async Task UpdatePerson_NullPerson_ToBeArgumentNullException()
    {
        PersonUpdateRequest? personUpdateRequest = null;

        var action = async () =>
        {
            await _personUpdaterService.UpdatePerson(personUpdateRequest);
        };

        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task UpdatePerson_InvalidPersonID_ToBeArgumentException()
    {
        var personUpdateRequest = new PersonUpdateRequest
        {
            PersonID = Guid.NewGuid()
        };

        var action = async () =>
        {
            await _personUpdaterService.UpdatePerson(personUpdateRequest);
        };

        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task UpdatePerson_PersonNameIsNull_ToBeArgumentException()
    {
        var person = _fixture.Build<Person>()
            .With(temp => temp.PersonName, null as string)
            .With(temp => temp.Email, "person@test.com")
            .With(temp => temp.Country, null as Country)
            .With(temp => temp.Gender, "Male")
            .Create();

        var personResponseFromAdd = person.ToPersonResponse();

        // Prepare a person update request
        var personUpdateRequest = personResponseFromAdd.ToPersonUpdateRequest();

        var action = async () =>
        {
            await _personUpdaterService.UpdatePerson(personUpdateRequest);
        };

        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task UpdatePerson_PersonDetails_ToBeSuccessful()
    {
        var person = _fixture.Build<Person>()
            .With(temp => temp.Email, "person@test.com")
            .With(temp => temp.Country, null as Country)
            .With(temp => temp.Gender, "Male")
            .Create();

        var personResponseExpected = person.ToPersonResponse();

        // Prepare a person update request with updated details
        var personUpdateRequest = personResponseExpected.ToPersonUpdateRequest();

        _personRepositoryMock
        .Setup(temp => temp.UpdatePerson(It.IsAny<Person>()))
        .ReturnsAsync(person);

        _personRepositoryMock
         .Setup(temp => temp.GetPersonByPersonID(It.IsAny<Guid>()))
         .ReturnsAsync(person);

        var personResponseFromUpdate = await _personUpdaterService.UpdatePerson(personUpdateRequest);

        // Assert the updated details are correct
        personResponseFromUpdate.Should().Be(personResponseExpected);
    }

    [Fact]
    public async Task DeletePerson_InvalidPersonID()
    {
        var PersonID = Guid.NewGuid();

        var isDeleted = await _personDeleterService.DeletePerson(PersonID);

        isDeleted.Should().BeFalse();
    }

    [Fact]
    public async Task DeletePerson_ValidPersonID_ToBeSuccessful()
    {
        // Create a person request and add it
        var person = _fixture.Build<Person>()
            .With(temp => temp.PersonName, "Person Test")
            .With(temp => temp.Email, "person@test.com")
            .With(temp => temp.Country, null as Country)
            .With(temp => temp.Gender, "Male")
            .Create();

        var personResponseFromAdd = person.ToPersonResponse();
        _personRepositoryMock.Setup(temp =>
            temp.DeletePersonByPersonID(It.IsAny<Guid>()))
            .ReturnsAsync(true);

        _personRepositoryMock.Setup(temp =>
            temp.GetPersonByPersonID(It.IsAny<Guid>()))
            .ReturnsAsync(person);

        // Delete the person
        var isDeleted = await _personDeleterService.DeletePerson(personResponseFromAdd.PersonID);

        // Assert the person was deleted successfully
        isDeleted.Should().BeTrue();
    }
}
