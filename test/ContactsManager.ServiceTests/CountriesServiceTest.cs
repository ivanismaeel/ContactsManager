using AutoFixture;
using Entities;
using FluentAssertions;
using Moq;
using RepositoryContracts;
using ServiceContracts;
using ServiceContracts.DTO;
using Services;

namespace CRUDTests;

public class CountriesServiceTest
{
    private readonly ICountriesGetterService _countriesGetterService;
    private readonly ICountriesAdderService _countriesAdderService;
    private readonly Mock<ICountriesRepository> _countriesRepositoryMock;
    private readonly ICountriesRepository _countriesRepository;

    private readonly IFixture _fixture;


    public CountriesServiceTest()
    {
        _fixture = new Fixture();

        _countriesRepositoryMock = new Mock<ICountriesRepository>();
        _countriesRepository = _countriesRepositoryMock.Object;
        _countriesGetterService = new CountriesGetterService(_countriesRepository);
        _countriesAdderService = new CountriesAdderService(_countriesRepository);
    }

    [Fact]
    public async Task AddCountry_NullCountry_ToBeArgumentNullException()
    {
        //Arrange
        CountryAddRequest? request = null;

        Country country = _fixture.Build<Country>()
             .With(temp => temp.Persons, null as List<Person>).Create();

        _countriesRepositoryMock
         .Setup(temp => temp.AddCountry(It.IsAny<Country>()))
         .ReturnsAsync(country);


        //Act
        var action = async () =>
        {
            await _countriesAdderService.AddCountry(request);
        };

        //Assert
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    //When the CountryName is null, it should throw ArgumentException
    [Fact]
    public async Task AddCountry_CountryNameIsNull_ToBeArgumentException()
    {
        //Arrange
        var request = _fixture.Build<CountryAddRequest>()
         .With(temp => temp.CountryName, null as string)
         .Create();

        var country = _fixture.Build<Country>()
             .With(temp => temp.Persons, null as List<Person>).Create();

        _countriesRepositoryMock
         .Setup(temp => temp.AddCountry(It.IsAny<Country>()))
         .ReturnsAsync(country);

        //Act
        var action = async () =>
        {
            await _countriesAdderService.AddCountry(request);
        };

        //Assert
        await action.Should().ThrowAsync<ArgumentException>();
    }


    //When the CountryName is duplicate, it should throw ArgumentException
    [Fact]
    public async Task AddCountry_DuplicateCountryName_ToBeArgumentException()
    {
        //Arrange
        var first_country_request = _fixture.Build<CountryAddRequest>()
             .With(temp => temp.CountryName, "Test name").Create();

        var second_country_request = _fixture.Build<CountryAddRequest>()
          .With(temp => temp.CountryName, "Test name").Create();

        var first_country = first_country_request.ToCountry();
        var second_country = second_country_request.ToCountry();

        _countriesRepositoryMock
         .Setup(temp => temp.AddCountry(It.IsAny<Country>()))
         .ReturnsAsync(first_country);

        //Return null when GetCountryByCountryName is called
        _countriesRepositoryMock
         .Setup(temp => temp.GetCountryByCountryName(It.IsAny<string>()))
         .ReturnsAsync(null as Country);

        var first_country_from_add_country = await _countriesAdderService.AddCountry(first_country_request);

        //Act
        var action = async () =>
        {
            //Return first country when GetCountryByCountryName is called
            _countriesRepositoryMock.Setup(temp => temp.AddCountry(It.IsAny<Country>())).ReturnsAsync(first_country);

            _countriesRepositoryMock.Setup(temp => temp.GetCountryByCountryName(It.IsAny<string>())).ReturnsAsync(first_country);

            await _countriesAdderService.AddCountry(second_country_request);
        };

        //Assert
        await action.Should().ThrowAsync<ArgumentException>();
    }

    //When you supply proper country name, it should insert (add) the country to the existing list of countries
    [Fact]
    public async Task AddCountry_FullCountry_ToBeSuccessful()
    {
        //Arrange
        var country_request = _fixture.Create<CountryAddRequest>();
        var country = country_request.ToCountry();
        var country_response = country.ToCountryResponse();

        _countriesRepositoryMock
         .Setup(temp => temp.AddCountry(It.IsAny<Country>()))
         .ReturnsAsync(country);

        _countriesRepositoryMock
         .Setup(temp => temp.GetCountryByCountryName(It.IsAny<string>()))
         .ReturnsAsync(null as Country);


        //Act
        var country_from_add_country = await _countriesAdderService.AddCountry(country_request);

        country.CountryID = country_from_add_country.CountryID;
        country_response.CountryID = country_from_add_country.CountryID;

        //Assert
        country_from_add_country.CountryID.Should().NotBe(Guid.Empty);
        country_from_add_country.Should().BeEquivalentTo(country_response);
    }

    [Fact]
    //The list of countries should be empty by default (before adding any countries)
    public async Task GetAllCountries_ToBeEmptyList()
    {
        //Arrange
        var country_empty_list = new List<Country>();
        _countriesRepositoryMock.Setup(temp => temp.GetAllCountries()).ReturnsAsync(country_empty_list);

        //Act
        var actual_country_response_list = await _countriesGetterService.GetAllCountries();

        //Assert
        actual_country_response_list.Should().BeEmpty();
    }


    [Fact]
    public async Task GetAllCountries_ShouldHaveFewCountries()
    {
        //Arrange
        var country_list = new List<Country>() {
        _fixture.Build<Country>()
        .With(temp => temp.Persons, null as List<Person>).Create(),
        _fixture.Build<Country>()
        .With(temp => temp.Persons, null as List<Person>).Create()
      };

        var country_response_list = country_list.Select(temp => temp.ToCountryResponse()).ToList();

        _countriesRepositoryMock.Setup(temp => temp.GetAllCountries()).ReturnsAsync(country_list);

        //Act
        var actualCountryResponseList = await _countriesGetterService.GetAllCountries();

        //Assert
        actualCountryResponseList.Should().BeEquivalentTo(country_response_list);
    }

    [Fact]
    //If we supply null as CountryID, it should return null as CountryResponse
    public async Task GetCountryByCountryID_NullCountryID_ToBeNull()
    {
        //Arrange
        Guid? countryID = null;

        _countriesRepositoryMock
         .Setup(temp => temp.GetCountryByCountryID(It.IsAny<Guid>()))
         .ReturnsAsync(null as Country);

        //Act
        var country_response_from_get_method = await _countriesGetterService.GetCountryByCountryID(countryID);


        //Assert
        country_response_from_get_method.Should().BeNull();
    }


    [Fact]
    //If we supply a valid country id, it should return the matching country details as CountryResponse object
    public async Task GetCountryByCountryID_ValidCountryID_ToBeSuccessful()
    {
        //Arrange
        var country = _fixture.Build<Country>()
          .With(temp => temp.Persons, null as List<Person>)
          .Create();
        var country_response = country.ToCountryResponse();

        _countriesRepositoryMock
         .Setup(temp => temp.GetCountryByCountryID(It.IsAny<Guid>()))
         .ReturnsAsync(country);

        //Act
        var country_response_from_get = await _countriesGetterService.GetCountryByCountryID(country.CountryID);

        //Assert
        country_response_from_get.Should().Be(country_response);
    }
}
