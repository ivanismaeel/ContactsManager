using System.Drawing;
using System.Globalization;
using CsvHelper.Configuration;
using Entities;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using RepositoryContracts;
using Serilog;
using SerilogTimings;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services;

public class PersonsGetterService(IPersonsRepository personsRepository,
    ILogger<PersonsGetterService?> logger, IDiagnosticContext diagnosticContext) : IPersonsGetterService
{
    public virtual async Task<List<PersonResponse>> GetAllPersons()
    {
        logger.LogInformation("Fetching all persons of personsService");

        var persons = await personsRepository.GetAllPersons();

        return [.. persons.Select(temp => temp.ToPersonResponse())];
    }

    public virtual async Task<PersonResponse?> GetPersonByPersonID(Guid? personID)
    {
        if (personID == null) return null;

        Person? person = await personsRepository.GetPersonByPersonID(personID.Value);

        if (person == null) return null;

        return person.ToPersonResponse();
    }

    public virtual async Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string? searchString)
    {
        logger.LogInformation("Fetching filtered persons in personsService");

        // if (string.IsNullOrEmpty(searchString)) return await GetAllPersons();

        var persons = new List<Person>();

        using (Operation.Time("Time for Filtered Persons from Database"))
        {
            var personsTask = searchBy switch
            {
                nameof(PersonResponse.PersonName) => personsRepository
                    .GetFilteredPersons(temp => temp.PersonName != null && temp.PersonName.Contains(searchString!)),

                nameof(PersonResponse.Email) => personsRepository
                    .GetFilteredPersons(temp => temp.Email != null && temp.Email.Contains(searchString!)),

                nameof(PersonResponse.DateOfBirth) => personsRepository
                    .GetFilteredPersons(temp => temp.DateOfBirth.HasValue),

                nameof(PersonResponse.Gender) => personsRepository
                    .GetFilteredPersons(temp => temp.Gender != null && temp.Gender.Contains(searchString!)),

                nameof(PersonResponse.CountryID) => personsRepository
                    .GetFilteredPersons(temp => temp.Country != null &&
                    temp.Country.CountryName != null &&
                    temp.Country.CountryName.Contains(searchString!)),

                nameof(PersonResponse.Address) => personsRepository
                    .GetFilteredPersons(temp => temp.Address != null && temp.Address.Contains(searchString!)),

                _ => personsRepository.GetAllPersons()
            };

            persons = await personsTask; // Await the task before applying further filtering

            if (searchBy == nameof(PersonResponse.DateOfBirth))
            {
                persons = [.. persons.Where(temp => temp.DateOfBirth!.Value.ToString("dd MMMM yyyy").Contains(searchString!))];
            }
        }

        diagnosticContext.Set("Persons", persons);

        return [.. persons.Select(temp => temp.ToPersonResponse())];
    }
    public virtual async Task<MemoryStream> GetPersonsCSV()
    {
        var persons = await GetAllPersons();
        var memoryStream = new MemoryStream();
        var streamWriter = new StreamWriter(memoryStream);

        var csvConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture);
        var csvWriter = new CsvHelper.CsvWriter(streamWriter, csvConfiguration);

        // onetime all data
        // csvWriter.WriteHeader<PersonResponse>();
        // await csvWriter.WriteRecordsAsync(persons);

        // or manually
        csvWriter.WriteField(nameof(PersonResponse.PersonName));
        csvWriter.WriteField(nameof(PersonResponse.Email));
        csvWriter.WriteField(nameof(PersonResponse.DateOfBirth));
        csvWriter.WriteField(nameof(PersonResponse.Age));
        csvWriter.WriteField(nameof(PersonResponse.Gender));
        csvWriter.WriteField(nameof(PersonResponse.Country));
        csvWriter.WriteField(nameof(PersonResponse.Address));
        csvWriter.WriteField(nameof(PersonResponse.ReceiveNewsLetters));
        csvWriter.NextRecord();

        //if write manually then need foreach
        foreach (var person in persons)
        {
            csvWriter.WriteField(person.PersonName);
            csvWriter.WriteField(person.Email);

            if (person.DateOfBirth.HasValue)
                csvWriter.WriteField(person.DateOfBirth.Value.ToString("yyyy-MM-dd"));
            else
                csvWriter.WriteField("");

            csvWriter.WriteField(person.Age);
            csvWriter.WriteField(person.Gender);
            csvWriter.WriteField(person.Country);
            csvWriter.WriteField(person.Address);
            csvWriter.WriteField(person.ReceiveNewsLetters);
            csvWriter.NextRecord();
        }

        memoryStream.Position = 0;
        return memoryStream;
    }

    public virtual async Task<MemoryStream> GetPersonsExcel()
    {
        var memoryStream = new MemoryStream();

        using (ExcelPackage excelPackage = new(memoryStream))
        {
            ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("PersonsSheet");

            worksheet.Cells["A1"].Value = "Person Name";
            worksheet.Cells["B1"].Value = "Email";
            worksheet.Cells["C1"].Value = "Date of Birth";
            worksheet.Cells["D1"].Value = "Age";
            worksheet.Cells["E1"].Value = "Gender";
            worksheet.Cells["F1"].Value = "Country";
            worksheet.Cells["G1"].Value = "Address";
            worksheet.Cells["H1"].Value = "Receive News Letters";

            using var range = worksheet.Cells["A1:H1"];
            range.Style.Font.Bold = true;
            range.Style.Fill.PatternType = ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
            range.Style.Font.Color.SetColor(Color.Black);
            range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
            range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            // range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            // range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            int row = 2;
            var persons = await GetAllPersons();

            foreach (var person in persons)
            {
                worksheet.Cells[row, 1].Value = person.PersonName;
                worksheet.Cells[row, 2].Value = person.Email;

                if (person.DateOfBirth.HasValue)
                    worksheet.Cells[row, 3].Value = person.DateOfBirth.Value.ToString("yyyy-MM-dd");

                worksheet.Cells[row, 4].Value = person.Age;
                worksheet.Cells[row, 5].Value = person.Gender;
                worksheet.Cells[row, 6].Value = person.Country;
                worksheet.Cells[row, 7].Value = person.Address;
                worksheet.Cells[row, 8].Value = person.ReceiveNewsLetters;

                row++;
            }

            worksheet.Cells[$"A1:H{row}"].AutoFitColumns();

            await excelPackage.SaveAsync();
        }

        memoryStream.Position = 0;
        return memoryStream;
    }
}
