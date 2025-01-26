using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.Text;

namespace Komit.Infrastructure.Spreadsheets.Implementation;
public class CsvService : ICsvService
{
    CsvReader csvReader;

    public async Task<MemoryStream> ConvertToCsv(string[][] columnValues, string[]? columnNames = default)
    {
        var stream = new MemoryStream();
        var csvWriter = await GetConfiguredCsvWriter(stream);

        //if (columnNames != default) WriteHeader(csvWriter, columnNames);
        WriteHeader(csvWriter, columnNames);  // Always header for now

        WriteRows(csvWriter, columnValues);

        await csvWriter.FlushAsync();
        stream.Position = 0;

        return stream;
    }

    private async Task<CsvWriter> GetConfiguredCsvWriter(MemoryStream stream)
    {
        var streamWriter = new StreamWriter(stream, Encoding.UTF8);
        var config = new CsvConfiguration(CultureInfo.CreateSpecificCulture("da-DK"));
        var csvWriter = new CsvWriter(streamWriter, config);

        return csvWriter;
    }

    private void WriteHeader(CsvWriter csvWriter, string[] columnNames)
    {
        foreach (var columnName in columnNames)
        {
            csvWriter.WriteField(columnName);
        }
        csvWriter.NextRecord();
    }

    private void WriteRows(CsvWriter csvWriter, string[][] columnValues)
    {
        foreach (var row in columnValues)
        {
            foreach (var cell in row)
            {
                csvWriter.WriteField(cell);
            }
            csvWriter.NextRecord();
        }
    }

    public IEnumerable<T> ReadCsv<T>(Func<string[], T> converter, int columns)
    {
        var result = new List<T>();
        var rowNumber = 1;

        while (csvReader.Read())
        {
            var row = new string[columns];
            for (int i = 0; i < columns; i++)
            {
                row[i] = csvReader.TryGetField<string>(i, out var field) ? field : default;
            }
            if (row.All(r => r.Equals(""))) continue; // Skip empty row.
            try
            {
                result.Add(converter(row));
            }
            catch (FormatException ex)
            {
                var fieldName = ex.Message;
                throw new Exception($"Fejl ved indlæsning af csv fil, i format af feltet {fieldName} på linje {rowNumber}. ");
            }
            catch (ArgumentException ex)
            {
                var fieldName = ex.Message;
                throw new Exception($"Fejl ved indlæsning af csv fil, ugyldig værdi i feltet {fieldName} på linje {rowNumber}. ");
            }
            rowNumber++;
        }

        return result;
    }

    public bool ValidateHeader(string[] columnDefinition)
    {
        csvReader.Read();

        for (var i = 0; i < columnDefinition.Length; i++)
        {
            var x = csvReader.TryGetField<string>(i, out var field) ? field : default;
            if (x == default || !x.Equals(columnDefinition[i]))
            {
                throw new Exception($"Fejl ved indlæsning af csv fil, ugyldig kolonne. Der bør stå {columnDefinition[i]} i kolonne nr. {i + 1}.");
            }
        }

        return true;
    }
    public void InitializeCsvReader(Stream stream)
    {
        csvReader = GetConfiguredCsvReader(stream);
    }

    private CsvReader GetConfiguredCsvReader(Stream stream)
    {
        var streamReader = new StreamReader(stream);
        var config = new CsvConfiguration(CultureInfo.CreateSpecificCulture("da-DK"));
        return new CsvReader(streamReader, config);
    }
}
