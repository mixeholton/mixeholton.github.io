namespace Komit.Infrastructure.Spreadsheets;
public interface IExcelService
{
    MemoryStream Write(IExcelTable headerTable, IExcelTable criteriaTable, IExcelTable contentTable);
    MemoryStream Write(IExcelTable headerTable, IExcelTable criteriaTable, IEnumerable<IExcelTable> contentTables);
    MemoryStream Write(IExcelTable headerTable, IExcelTable criteriaTable, params IExcelTable[] contentTables);
}
public interface IExcelColumn
{
    string Name { get; }
    ExcelStyle Style { get; }
    bool Paint { get; }
    int? cellMergeIndex { get; }
    bool Bold { get; }
}
public interface IExcelTable
{
    IEnumerable<IExcelColumn> Columns { get; }
    IEnumerable<IEnumerable<dynamic>> RowsWithCells { get; }
    bool AddEmptyRowAfter { get; }
    bool AddFilterIfLast { get; }
    IExcelTableOrientation Orientation { get; }
    bool AddHeader { get; }
}
public enum ExcelStyle
{
    Text,
    Date,
    DateTime,
    Currency,
    Number
}
public enum IExcelTableOrientation
{
    Vertical,
    Horizontal
}