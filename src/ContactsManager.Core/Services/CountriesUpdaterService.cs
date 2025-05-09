using Entities;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using RepositoryContracts;
using ServiceContracts;

namespace Services;

public class CountriesUpdaterService(ICountriesRepository countriesRepository) : ICountriesUpdaterService
{
    public async Task<int> UploadCountriesFromExcelFile(IFormFile formFile)
    {
        var memoryStream = new MemoryStream();
        await formFile.CopyToAsync(memoryStream);

        using var excelPackage = new ExcelPackage(memoryStream);

        var worksheet = excelPackage.Workbook.Worksheets["Countries"]
            ?? throw new Exception("Worksheet 'Countries' not found.");

        var rowCount = worksheet.Dimension.Rows;
        int insertedCount = 0;

        for (int row = 2; row <= rowCount; row++) // Skip header row
        {
            var cellValue = Convert.ToString(worksheet.Cells[row, 1].Value);

            if (!string.IsNullOrEmpty(cellValue))
            {
                var countryName = cellValue.Trim();

                // Asynchronously check if country exists in the database
                if (await countriesRepository.GetCountryByCountryName(countryName) == null)
                {
                    var country = new Country
                    {
                        CountryName = countryName
                    };

                    // Add each country individually
                    await countriesRepository.AddCountry(country);
                    insertedCount++;
                }
            }
        }

        return insertedCount; // Return number of countries inserted
    }

}
