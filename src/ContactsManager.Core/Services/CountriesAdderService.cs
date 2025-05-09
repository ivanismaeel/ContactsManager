using Entities;
using RepositoryContracts;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services;

public class CountriesAdderService(ICountriesRepository countriesRepository) : ICountriesAdderService
{
    public async Task<CountryResponse> AddCountry(CountryAddRequest? request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request), "Country details are required.");

        if (string.IsNullOrWhiteSpace(request.CountryName))
            throw new ArgumentNullException(nameof(request.CountryName), "Country name is required.");

        if (await countriesRepository.GetCountryByCountryName(request.CountryName) != null)
            throw new ArgumentException("Country name already exists.");

        Country country = (request?.ToCountry())
            ?? throw new ArgumentNullException(nameof(request), "Country details are required.");

        country.CountryID = Guid.NewGuid();

        await countriesRepository.AddCountry(country);

        return country.ToCountryResponse();
    }
}
