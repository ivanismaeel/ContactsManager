using RepositoryContracts;
using ServiceContracts;
using ServiceContracts.DTO;
using Services.Helpers;

namespace Services;

public class PersonsAdderService(IPersonsRepository personsRepository) : IPersonsAdderService
{
    public async Task<PersonResponse> AddPerson(PersonAddRequest? request)
    {
        ArgumentNullException.ThrowIfNull(request);

        ValidationHelper.ModelValidation(request);

        var person = request.ToPerson();
        person.PersonID = Guid.NewGuid();

        await personsRepository.AddPerson(person);

        return person.ToPersonResponse();
    }
}
