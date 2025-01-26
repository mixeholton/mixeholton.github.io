namespace Komit.Infrastructure.Spreadsheets;
public record ExcelColumn<T>(string Name, ExcelStyle Style, Func<T, dynamic> CellFunction, bool Paint = false, bool Bold = false) : IExcelColumn
{
    public int? cellMergeIndex { get; set; }
}
public record ExcelColumnLine(string Name, ExcelStyle Style, bool Paint = false, bool Bold = false) : IExcelColumn
{
    public int? cellMergeIndex { get; set; }
}
public record ExcelLine(IEnumerable<ExcelColumnLine> Lines, bool AddEmptyRowAfter = true, IExcelTableOrientation Orientation = IExcelTableOrientation.Horizontal,
    bool AddHeader = true) : IExcelTable
{
    bool IExcelTable.AddFilterIfLast => false;
    IEnumerable<IExcelColumn> IExcelTable.Columns => Lines;
    IEnumerable<IEnumerable<dynamic>> IExcelTable.RowsWithCells => Array.Empty<IEnumerable<dynamic>>();
}
public record ExcelTable<T>(IEnumerable<ExcelColumn<T>> ColumnDefinitions, IEnumerable<T> Records, bool AddEmptyRowAfter = true, bool AddFilterIfLast = false,
    IExcelTableOrientation Orientation = IExcelTableOrientation.Horizontal, bool AddHeader = true) : IExcelTable
{
    IEnumerable<IExcelColumn> IExcelTable.Columns => ColumnDefinitions;
    IEnumerable<IEnumerable<dynamic>> IExcelTable.RowsWithCells => Records.Select(record => ColumnDefinitions.Select(column => column.CellFunction(record)));
}
public record ReportHeader(string OrganizationName, string UserName, string ReportName, DateTime ReportingTime);
public record ExcelHeaderTable(ReportHeader Header) : ExcelTable<ReportHeader>(HeaderColumns(), new [] { Header })
{
    public static IEnumerable<ExcelColumn<ReportHeader>> HeaderColumns() => new ExcelColumn<ReportHeader>[]
    {
        new ("Organisation", ExcelStyle.Text, x => x.OrganizationName),
        new ("Bruger", ExcelStyle.Text, x => x.UserName),
        new ("Udskrift", ExcelStyle.Text, x => x.ReportName),
        new ("Tidspunkt", ExcelStyle.DateTime, x => x.ReportingTime),
    };
};
