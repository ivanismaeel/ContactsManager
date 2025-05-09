using ServiceContracts.DTO;

namespace ServiceContracts;

public interface IPersonsGetterService
{
    public Task<List<PersonResponse>> GetAllPersons();
    Task<PersonResponse?> GetPersonByPersonID(Guid? PersonID);
    Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string? searchString);
    Task<MemoryStream> GetPersonsCSV();
    Task<MemoryStream> GetPersonsExcel();
}
