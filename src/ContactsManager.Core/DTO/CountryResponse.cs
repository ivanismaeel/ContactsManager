using Entities;

namespace ServiceContracts.DTO;

public class CountryResponse
{
    public Guid CountryID { get; set; }
    public string? CountryName { get; set; }

    public override bool Equals(object? obj)
    {
        if (obj == null) return false;

        if (obj.GetType() != typeof(CountryResponse)) return false;

        CountryResponse country_to_compare = (CountryResponse)obj;

        return CountryID == country_to_compare.CountryID && CountryName == country_to_compare.CountryName;
    }

    public override int GetHashCode()
    {
        throw new NotImplementedException();
    }
}

public static class CountryResponseExtensions
{
    public static CountryResponse ToCountryResponse(this Country country)
    {
        return new CountryResponse
        {
            CountryID = country.CountryID,
            CountryName = country.CountryName
        };
    }
}