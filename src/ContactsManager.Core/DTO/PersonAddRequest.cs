using System.ComponentModel.DataAnnotations;
using Entities;
using ServiceContracts.Enums;

namespace ServiceContracts.DTO;

public class PersonAddRequest
{
    [Required(ErrorMessage = "Person Name can't be blank")]
    public string? PersonName { get; set; }

    [Required(ErrorMessage = "Email can't be blank")]
    [EmailAddress(ErrorMessage = "Email value should be a valid email")]
    [DataType(DataType.EmailAddress)]
    public string? Email { get; set; }

    [DataType(DataType.Date)]
    public DateTime? DateOfBirth { get; set; }

    [Required(ErrorMessage = "Please select a gender")]
    public GenderOptions? Gender { get; set; }

    [Required(ErrorMessage = "Please select a country")]
    public Guid? CountryID { get; set; }
    public string? Address { get; set; }
    public bool ReceiveNewsLetters { get; set; }

    public Person ToPerson()
    {
        return new()
        {
            PersonName = PersonName,
            Email = Email,
            DateOfBirth = DateOfBirth,
            Gender = Gender.ToString(),
            Address = Address,
            CountryID = CountryID,
            ReceiveNewsLetters = ReceiveNewsLetters
        };
    }
}
