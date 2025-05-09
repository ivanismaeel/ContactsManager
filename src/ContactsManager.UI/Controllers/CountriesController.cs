using Microsoft.AspNetCore.Mvc;
using ServiceContracts;

namespace CRUD.NET.Controllers;

[Route("[controller]")]
public class CountriesController(ICountriesUpdaterService countriesUpdaterService) : Controller
{
    [Route("UploadFromExcel")]
    public IActionResult UploadFromExcel()
    {
        return View();
    }

    [HttpPost]
    [Route("UploadFromExcel")]
    public async Task<IActionResult> UploadFromExcelAsync(IFormFile excelFile)
    {
        if (excelFile == null || excelFile.Length == 0)
        {
            ViewBag.Message = "Please select an xlsx file to upload.";
            return View();
        }

        if (!Path.GetExtension(excelFile.FileName).Equals(".xlsx",
            StringComparison.OrdinalIgnoreCase))
        {
            ViewBag.Message = "Unsupported file format. 'xlsx' file is expected.";
            return View();
        }

        var countries = await countriesUpdaterService.UploadCountriesFromExcelFile(excelFile);
        ViewBag.Message = $"Successfully uploaded {countries} countries.";

        return View();
    }
}
