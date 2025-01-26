namespace Komit.Infrastructure.Spreadsheets;
public interface ICsvService
{
    Task<MemoryStream> ConvertToCsv(string[][] columnValues, string[]? columnNames = null);
    IEnumerable<T> ReadCsv<T>(Func<string[], T> converter, int columns);
    void InitializeCsvReader(Stream stream);
    bool ValidateHeader(string[] columnDefinition);
}

