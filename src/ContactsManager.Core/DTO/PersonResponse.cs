using Entities;
using ServiceContracts.Enums;

namespace ServiceContracts.DTO;

public class PersonResponse
{
    public Guid PersonID { get; set; }
    public string? PersonName { get; set; }
    public string? Email { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public Guid? CountryID { get; set; }
    public string? Country { get; set; }
    public string? Address { get; set; }
    public bool ReceiveNewsLetters { get; set; }
    public double? Age { get; set; }

    public override bool Equals(object? obj)
    {
        if (obj == null || GetType() != obj.GetType()) return false;

        PersonResponse person = (PersonResponse)obj;

        return PersonID == person.PersonID
            && string.Equals(PersonName, person.PersonName, StringComparison.OrdinalIgnoreCase)
            && string.Equals(Email, person.Email, StringComparison.OrdinalIgnoreCase)
            && Nullable.Equals(DateOfBirth, person.DateOfBirth)
            && string.Equals(Gender, person.Gender, StringComparison.OrdinalIgnoreCase)
            && CountryID == person.CountryID
            && string.Equals(Country, person.Country, StringComparison.OrdinalIgnoreCase)
            && string.Equals(Address, person.Address, StringComparison.OrdinalIgnoreCase)
            && ReceiveNewsLetters == person.ReceiveNewsLetters
            && Age == person.Age;
    }





    public override string ToString()
    {
        return $"Person ID: {PersonID}, Name: {PersonName ?? "N/A"}, Email: {Email ?? "N/A"}, DOB: {DateOfBirth?.ToString("dd MMM yyyy") ?? "N/A"}, Gender: {Gender ?? "N/A"}, Country: {Country ?? "N/A"}, Address: {Address ?? "N/A"}, Receive Newsletters: {ReceiveNewsLetters}, Age: {Age?.ToString("F1") ?? "N/A"}";
    }


    public PersonUpdateRequest ToPersonUpdateRequest()
    {
        return new()
        {
            PersonID = PersonID,
            PersonName = PersonName,
            Email = Email,
            DateOfBirth = DateOfBirth,
            Gender = !string.IsNullOrEmpty(Gender) ? Enum.Parse<GenderOptions>(Gender, true) : GenderOptions.Other,
            CountryID = CountryID,
            Address = Address,
            ReceiveNewsLetters = ReceiveNewsLetters
        };
    }

    public override int GetHashCode()
    {
        throw new NotImplementedException();
    }
}

public static class PersonExtensions
{
    public static PersonResponse ToPersonResponse(this Person person)
    {
        return new()
        {
            PersonID = person.PersonID,
            PersonName = person.PersonName,
            Email = person.Email,
            DateOfBirth = person.DateOfBirth,
            ReceiveNewsLetters = person.ReceiveNewsLetters,
            Address = person.Address,
            CountryID = person.CountryID,
            Gender = person.Gender,
            Age = (person.DateOfBirth != null)
                ? Math.Round((DateTime.Now - person.DateOfBirth.Value).TotalDays / 365.25)
                : null,
            Country = person.Country?.CountryName
        };
    }
}