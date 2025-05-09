using Entities;
using RepositoryContracts;
using ServiceContracts;

namespace Services;

public class PersonsDeleterService(IPersonsRepository personsRepository) : IPersonsDeleterService
{
    public async Task<bool> DeletePerson(Guid? personID)
    {
        if (personID == null) throw new ArgumentNullException(nameof(personID));

        Person? person = await personsRepository.GetPersonByPersonID(personID.Value);

        if (person == null) return false;

        await personsRepository.DeletePersonByPersonID(personID.Value);

        return true;
    }
}
