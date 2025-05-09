using Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;

namespace Repositories;

public class CountriesRepository(ApplicationDbContext db) : ICountriesRepository
{
    public async Task<Country> AddCountry(Country country)
    {
        db.Countries.Add(country);
        await db.SaveChangesAsync();

        return country;
    }

    public async Task<List<Country>> GetAllCountries()
    {
        return await db.Countries.ToListAsync();
    }

    public async Task<Country?> GetCountryByCountryID(Guid CountryID)
    {
        return await db.Countries.FirstOrDefaultAsync(temp => temp.CountryID == CountryID);
    }

    public async Task<Country?> GetCountryByCountryName(string countryName)
    {
        return await db.Countries.FirstOrDefaultAsync(temp => temp.CountryName == countryName);
    }
}
