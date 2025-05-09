using Microsoft.AspNetCore.Http;

namespace ServiceContracts;

public interface ICountriesUpdaterService
{

    Task<int> UploadCountriesFromExcelFile(IFormFile formFile);
}
