using RepositoryContracts;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services;

public class CountriesGetterService(ICountriesRepository countriesRepository) : ICountriesGetterService
{
    public async Task<List<CountryResponse>> GetAllCountries()
    {
        return [.. (await countriesRepository.GetAllCountries())
            .Select(country => country.ToCountryResponse())];
    }

    public async Task<CountryResponse?> GetCountryByCountryID(Guid? countryID)
    {
        if (countryID == null) return null;

        var country_response_from_list = await countriesRepository.GetCountryByCountryID(countryID.Value);

        if (country_response_from_list == null) return null;

        return country_response_from_list.ToCountryResponse();
    }
}
