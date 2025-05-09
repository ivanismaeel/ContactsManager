using ServiceContracts.DTO;

namespace ServiceContracts;

public interface IPersonsAdderService
{
    public Task<PersonResponse> AddPerson(PersonAddRequest? request);
}
