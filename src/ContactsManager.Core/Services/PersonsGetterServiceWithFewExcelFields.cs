using System.Drawing;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services;

public class PersonsGetterServiceWithFewExcelFields(PersonsGetterService getterService) : IPersonsGetterService
{
    public async Task<List<PersonResponse>> GetAllPersons()
    {
        return await getterService.GetAllPersons();
    }

    public async Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string? searchString)
    {
        return await getterService.GetFilteredPersons(searchBy, searchString);
    }

    public async Task<PersonResponse?> GetPersonByPersonID(Guid? PersonID)
    {
        return await getterService.GetPersonByPersonID(PersonID);
    }

    public async Task<MemoryStream> GetPersonsCSV()
    {
        return await getterService.GetPersonsCSV();
    }

    public async Task<MemoryStream> GetPersonsExcel()
    {
        var memoryStream = new MemoryStream();

        using (ExcelPackage excelPackage = new(memoryStream))
        {
            ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("PersonsSheet");

            worksheet.Cells["A1"].Value = "Person Name";
            worksheet.Cells["B1"].Value = "Age";
            worksheet.Cells["C1"].Value = "Gender";

            using var range = worksheet.Cells["A1:C1"];
            range.Style.Font.Bold = true;
            range.Style.Fill.PatternType = ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
            range.Style.Font.Color.SetColor(Color.Black);
            range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
            range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

            int row = 2;
            var persons = await GetAllPersons();

            foreach (var person in persons)
            {
                worksheet.Cells[row, 1].Value = person.PersonName;
                worksheet.Cells[row, 2].Value = person.Age;
                worksheet.Cells[row, 3].Value = person.Gender;

                row++;
            }

            worksheet.Cells[$"A1:C{row}"].AutoFitColumns();

            await excelPackage.SaveAsync();
        }

        memoryStream.Position = 0;
        return memoryStream;
    }
}
