using Entities;
using Exceptions;
using RepositoryContracts;
using ServiceContracts;
using ServiceContracts.DTO;
using Services.Helpers;

namespace Services;

public class PersonsUpdaterService(IPersonsRepository personsRepository) : IPersonsUpdaterService
{
    public async Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest)
    {
        ArgumentNullException.ThrowIfNull(personUpdateRequest);

        Person? matchingPerson = await personsRepository.GetPersonByPersonID(personUpdateRequest.PersonID)
            ?? throw new InvalidPersonIDException("Given person id doesn't exist");

        ValidationHelper.ModelValidation(personUpdateRequest);

        matchingPerson.PersonName = personUpdateRequest.PersonName;
        matchingPerson.Email = personUpdateRequest.Email;
        matchingPerson.DateOfBirth = personUpdateRequest.DateOfBirth;
        matchingPerson.Gender = personUpdateRequest.Gender.ToString();
        matchingPerson.CountryID = personUpdateRequest.CountryID;
        matchingPerson.Address = personUpdateRequest.Address;
        matchingPerson.ReceiveNewsLetters = personUpdateRequest.ReceiveNewsLetters;

        await personsRepository.UpdatePerson(matchingPerson);

        return matchingPerson.ToPersonResponse();
    }
}
