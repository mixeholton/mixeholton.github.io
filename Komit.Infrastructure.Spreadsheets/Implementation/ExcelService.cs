using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;

namespace Komit.Infrastructure.Spreadsheets.Implementation;
public class ExcelService : IExcelService
{
    public MemoryStream Write(IExcelTable headerTable, IExcelTable criteriaTable, IExcelTable contentTable)
        => Write(headerTable, criteriaTable, new[] { contentTable }.AsEnumerable());
    public MemoryStream Write(IExcelTable headerTable, IExcelTable criteriaTable, params IExcelTable[] contentTables)
        => Write(headerTable, criteriaTable, contentTables.AsEnumerable());
    public MemoryStream Write(IExcelTable headerTable, IExcelTable criteriaTable, IEnumerable<IExcelTable> contentTables)
    {
        var workbook = new XSSFWorkbook();
        var sheet = workbook.CreateSheet();
        var format = workbook.CreateDataFormat();
        IReadOnlyDictionary<ExcelStyle, ICellStyle> CellStyleIndex = new Dictionary<ExcelStyle, ICellStyle>()
        {
            [ExcelStyle.Text] = CreateTextStyle(),
            [ExcelStyle.Date] = CreateDateStyle(),
            [ExcelStyle.DateTime] = CreateDateTimeStyle(),
            [ExcelStyle.Currency] = CreateCurrencyStyle(),
            [ExcelStyle.Number] = CreateNumberStyle()
        };
        IReadOnlyDictionary<ExcelStyle, ICellStyle> CellStylePaintedIndex = new Dictionary<ExcelStyle, ICellStyle>()
        {
            [ExcelStyle.Text] = CreateTextStyle(true),
            [ExcelStyle.Date] = CreateDateStyle(true),
            [ExcelStyle.DateTime] = CreateDateTimeStyle(true),
            [ExcelStyle.Currency] = CreateCurrencyStyle(true),
            [ExcelStyle.Number] = CreateNumberStyle(true)
        };
        IReadOnlyDictionary<ExcelStyle, ICellStyle> CellStyleBoldIndex = new Dictionary<ExcelStyle, ICellStyle>()
        {
            [ExcelStyle.Text] = CreateTextStyle(painted: false, bold: true),
            [ExcelStyle.Date] = CreateDateStyle(painted: false, bold: true),
            [ExcelStyle.DateTime] = CreateDateTimeStyle(painted: false, bold: true),
            [ExcelStyle.Currency] = CreateCurrencyStyle(painted: false, bold: true),
            [ExcelStyle.Number] = CreateNumberStyle(painted: false, bold: true)
        };
        IReadOnlyDictionary<ExcelStyle, ICellStyle> CellStylePaintedBoldIndex = new Dictionary<ExcelStyle, ICellStyle>()
        {
            [ExcelStyle.Text] = CreateTextStyle(painted: true, bold: true),
            [ExcelStyle.Date] = CreateDateStyle(painted: true, bold: true),
            [ExcelStyle.DateTime] = CreateDateTimeStyle(painted: true, bold: true),
            [ExcelStyle.Currency] = CreateCurrencyStyle(painted: true, bold: true),
            [ExcelStyle.Number] = CreateNumberStyle(painted: true, bold: true)
        };
        ICellStyle GetCellStyle(IExcelColumn column)
        {
            var style = CellStyleIndex[column.Style];

            if (column.Paint)
            {
                if (column.Bold)
                {
                    style = CellStylePaintedBoldIndex[column.Style];
                }
                else
                {
                    style = CellStylePaintedIndex[column.Style];
                }
                    
            }
            else if (column.Bold)
            {
                style = CellStyleBoldIndex[column.Style];
            }
            return style; 
        }

        int rowIndex = 0;
        var headerStyle = HeaderStyle();
        var headerStylePainted = HeaderStyle(true);
        var nrOfColumns = contentTables.Last().Columns.Count();

        WriteTable(headerTable);
        if (criteriaTable != null)
            WriteTable(criteriaTable);
        foreach (var table in contentTables)
            WriteTable(table);
        var lastTable = contentTables.Last();
        if (lastTable.RowsWithCells.Any() && (contentTables.Count() == 1 || lastTable.AddFilterIfLast))
        {
            var filterIndex = rowIndex - lastTable.RowsWithCells.Count() - (lastTable.AddEmptyRowAfter ? 2 : 1);
            sheet.SetAutoFilter(new CellRangeAddress(filterIndex, filterIndex, 0, nrOfColumns - 1));
        }

        for (int i = 0; i < nrOfColumns; i++)
        {
            sheet.AutoSizeColumn(i);
            if (sheet.GetColumnWidth(i) < 2560)
                sheet.SetColumnWidth(i, 2560);
            if (sheet.GetColumnWidth(i) > 25600)
                sheet.SetColumnWidth(i, 25600);
        }
        return WriteToStream(workbook);

        void WriteTable (IExcelTable table)
        {
            if (table.Orientation == IExcelTableOrientation.Horizontal)
            {
                var startRowIndex = rowIndex;
                if (table.AddHeader)
                {
                    WriteTableColumnHeader(table.Columns);
                }
                
                WriteTableRows(table);
                MergeCells(startRowIndex, rowIndex - 1, table.Columns);
            }
            else if (table.Orientation == IExcelTableOrientation.Vertical)
            {
                WriteVerticalTable(table);
            }
            if (table.AddEmptyRowAfter)
                rowIndex++;

        }
        void WriteTableColumnHeader(IEnumerable<IExcelColumn> columns)
        {
            var headerRow = sheet.CreateRow(rowIndex++);
            var cellIndex = 0;
            foreach (var column in columns)
            {
                var headerCell = headerRow.CreateCell(cellIndex++);
                if (column.Name != null)
                    headerCell.SetCellValue(column.Name);
                headerCell.CellStyle = column.Paint ? headerStylePainted : headerStyle;
            }
        }
        void WriteTableRows(IExcelTable table)
        {
            foreach (var record in table.RowsWithCells)
            {
                var row = sheet.CreateRow(rowIndex++);
                var cells = record.ToArray();
                var cellIndex = 0;
                foreach (var column in table.Columns)
                {
                    var cell = row.CreateCell(cellIndex);
                    var cellValue = cells[cellIndex];
                    if (cellValue != null)
                        cell.SetCellValue(cellValue);
                    cell.CellStyle = GetCellStyle(column);
                    cellIndex++;
                }
            }
        }
        void MergeCells(int startRow, int endRow, IEnumerable<IExcelColumn> columns)
        {
            if (!columns.Any(x => x.cellMergeIndex.HasValue) || startRow > endRow)
                return;
            var currentMergeIndex = -1;
            var startCellIndex = -1;
            for (int i = 0; i < columns.Count(); i++)
            {
                var column = columns.ElementAt(i);
                if (currentMergeIndex >= 0 && (!column.cellMergeIndex.HasValue || column.cellMergeIndex.Value != currentMergeIndex))
                { // End cell merge range
                    var endCellIndex = i - 1;
                    if (startCellIndex < endCellIndex)
                        sheet.AddMergedRegion(new CellRangeAddress(startRow, endRow, startCellIndex, endCellIndex));
                    currentMergeIndex = -1;
                    startCellIndex = -1;
                }
                if (currentMergeIndex < 0 && column.cellMergeIndex.HasValue)
                {  // Start cell merge range
                    currentMergeIndex = column.cellMergeIndex.Value;
                    startCellIndex = i;
                }
            }
            if (currentMergeIndex >= 0)
            {
                var endCellIndex = columns.Count() - 1;
                if (startCellIndex < endCellIndex)
                    sheet.AddMergedRegion(new CellRangeAddress(startRow, endRow, startCellIndex, endCellIndex));
            }
        }
        void WriteVerticalTable(IExcelTable table)
        {
            var rows = new List<IRow>();
            var cellIndex = 0;
            var columns = table.Columns;
            var startRowIndex = rowIndex;

            // Vertical header
            foreach (var column in columns.Select((column, index) => (column, index)))
            {
                rows.Add(sheet.CreateRow(rowIndex++));
                var headerCell = rows[column.index].CreateCell(cellIndex);
                if (column.column.Name != null)
                    headerCell.SetCellValue(column.column.Name);
                headerCell.CellStyle = column.column.Paint ? headerStylePainted : headerStyle;
            }
            cellIndex++;

            // Vertical "rows"
            foreach (var record in table.RowsWithCells)
            {
                var cells = record.ToArray();
                foreach (var column in table.Columns.Select((column, index) => (column, index)))
                {
                    var row = rows[column.index];
                    var cell = row.CreateCell(cellIndex);
                    var cellValue = cells[column.index];
                    if (cellValue != null)
                        cell.SetCellValue(cellValue);
                    cell.CellStyle = GetCellStyle(column.column);
                }
                cellIndex++;
            }
        }
        MemoryStream WriteToStream(IWorkbook workbook)
        {
            var resultStream = new MemoryStream();
            using (var stream = new MemoryStream())
            {
                workbook.Write(stream, false);
                var byteArray = stream.ToArray();
                resultStream.Write(byteArray, 0, byteArray.Length);
                resultStream.Seek(0, SeekOrigin.Begin);
            }
            return resultStream;
        }
        ICellStyle CreateTitleStyle()
        {
            var style = workbook.CreateCellStyle();
            var font = workbook.CreateFont();
            font.IsBold = true;
            font.FontHeightInPoints = 18;
            style.SetFont(font);
            return style;
        }
        ICellStyle HeaderStyle(bool painted = false)
        {
            var style = workbook.CreateCellStyle();
            var font = workbook.CreateFont();
            font.IsBold = true;
            font.FontHeightInPoints++;
            style.SetFont(font);
            style.Alignment = HorizontalAlignment.Center;
            style.VerticalAlignment = VerticalAlignment.Center;
            if (painted)
                PaintCellStyle(style);
            return style;
        }
        ICellStyle CreateTextStyle(bool painted = false, bool bold = false)
        {
            var style = workbook.CreateCellStyle();
            style.DataFormat = HSSFDataFormat.GetBuiltinFormat("@");

            if (painted)
            {
                PaintCellStyle(style);
            }
            if (bold)
            {
                BoldCellStyle(style);
            }
                
            return style;
        }
        ICellStyle CreateDateStyle(bool painted = false, bool bold = false)
        {
            var style = workbook.CreateCellStyle();
            style.DataFormat = format.GetFormat("dd-MM-yyyy");

            if (painted)
            {
                PaintCellStyle(style);
            }
            if (bold)
            {
                BoldCellStyle(style);
            }

            return style;
        }
        ICellStyle CreateDateTimeStyle(bool painted = false, bool bold = false)
        {
            var style = workbook.CreateCellStyle();
            style.DataFormat = format.GetFormat("dd-MM-yyyy hh:mm:ss");

            if (painted)
            {
                PaintCellStyle(style);
            }
            if (bold)
            {
                BoldCellStyle(style);
            }

            return style;
        }
        ICellStyle CreateCurrencyStyle(bool painted = false, bool bold = false)
        {
            var style = workbook.CreateCellStyle();
            style.DataFormat = format.GetFormat("#,##0.00");

            if (painted)
            {
                PaintCellStyle(style);
            }
            if (bold)
            {
                BoldCellStyle(style);
            }

            return style;
        }
        ICellStyle CreateNumberStyle(bool painted = false, bool bold = false)
        {
            var style = workbook.CreateCellStyle();

            if (painted)
            {
                PaintCellStyle(style);
            }
            if (bold)
            {
                BoldCellStyle(style);
            }

            return style;
        }
        void PaintCellStyle(ICellStyle cellStyle)
        {
            cellStyle.BorderBottom = BorderStyle.Thin;
            cellStyle.BottomBorderColor = IndexedColors.Grey50Percent.Index;
            cellStyle.BorderLeft = BorderStyle.Thin;
            cellStyle.LeftBorderColor = IndexedColors.Grey50Percent.Index;
            cellStyle.BorderRight = BorderStyle.Thin;
            cellStyle.RightBorderColor = IndexedColors.Grey50Percent.Index;
            cellStyle.BorderTop = BorderStyle.Thin;
            cellStyle.TopBorderColor = IndexedColors.Grey50Percent.Index;
            cellStyle.FillPattern = FillPattern.SolidForeground;
            cellStyle.FillForegroundColor = IndexedColors.LightGreen.Index;
        }
        void BoldCellStyle(ICellStyle cellStyle)
        {
            var font = workbook.CreateFont();
            font.IsBold = true;
            cellStyle.SetFont(font);
        }
    }
}