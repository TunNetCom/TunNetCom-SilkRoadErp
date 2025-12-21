using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Text;

namespace TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services;

public class ExcelExportService
{
    private readonly ILogger<ExcelExportService> _logger;

    public ExcelExportService(ILogger<ExcelExportService> logger)
    {
        _logger = logger;
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
    }

    public byte[] ExportToExcel<T>(
        IEnumerable<T> data,
        List<ColumnMapping> columns,
        string sheetName = "Export",
        int decimalPlaces = 3,
        decimal? totalNetAmount = null,
        decimal? totalVatAmount = null,
        decimal? totalTtcAmount = null)
    {
        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add(sheetName);

        // Check if we need to add TTC column
        var hasNetAmount = columns.Any(c => c.PropertyName.Equals("NetAmount", StringComparison.OrdinalIgnoreCase));
        var hasVatAmount = columns.Any(c => c.PropertyName.Equals("VatAmount", StringComparison.OrdinalIgnoreCase));
        var addTtcColumn = hasNetAmount && hasVatAmount;

        // Set headers
        var headerRow = 1;
        var colIndex = 1;
        foreach (var column in columns)
        {
            worksheet.Cells[headerRow, colIndex].Value = column.DisplayName;
            worksheet.Cells[headerRow, colIndex].Style.Font.Bold = true;
            worksheet.Cells[headerRow, colIndex].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[headerRow, colIndex].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            worksheet.Cells[headerRow, colIndex].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            colIndex++;
        }
        
        // Add TTC column header if needed
        if (addTtcColumn)
        {
            worksheet.Cells[headerRow, colIndex].Value = "Total TTC";
            worksheet.Cells[headerRow, colIndex].Style.Font.Bold = true;
            worksheet.Cells[headerRow, colIndex].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[headerRow, colIndex].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            worksheet.Cells[headerRow, colIndex].Style.Border.BorderAround(ExcelBorderStyle.Thin);
        }

        // Set data rows
        var rowIndex = 2;
        foreach (var item in data)
        {
            colIndex = 1;
            foreach (var column in columns)
            {
                var value = GetPropertyValue(item, column.PropertyName);
                var cell = worksheet.Cells[rowIndex, colIndex];
                
                // Format based on property type
                if (value is decimal || value is double || value is float)
                {
                    cell.Value = value;
                    // Format with specified decimal places (default 3 for French format)
                    var formatString = decimalPlaces == 3 ? "#,##0.000" : $"#,##0.{new string('0', decimalPlaces)}";
                    cell.Style.Numberformat.Format = formatString;
                }
                else if (value is DateTime || value is DateTimeOffset)
                {
                    var dateValue = value is DateTimeOffset dto ? dto.DateTime : (DateTime)value;
                    cell.Value = dateValue;
                    cell.Style.Numberformat.Format = "dd/MM/yyyy";
                }
                else if (value is int || value is long)
                {
                    cell.Value = value;
                    cell.Style.Numberformat.Format = "0";
                }
                else
                {
                    cell.Value = value?.ToString() ?? string.Empty;
                }

                cell.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                colIndex++;
            }
            
            // Add TTC column if needed
            if (addTtcColumn)
            {
                var netAmount = GetPropertyValue(item, "NetAmount");
                var vatAmount = GetPropertyValue(item, "VatAmount");
                decimal ttcValue = 0;
                if (netAmount is decimal net && vatAmount is decimal vat)
                {
                    ttcValue = net + vat;
                }
                
                var ttcCell = worksheet.Cells[rowIndex, colIndex];
                ttcCell.Value = ttcValue;
                var formatString = decimalPlaces == 3 ? "#,##0.000" : $"#,##0.{new string('0', decimalPlaces)}";
                ttcCell.Style.Numberformat.Format = formatString;
                ttcCell.Style.Border.BorderAround(ExcelBorderStyle.Thin);
            }
            
            rowIndex++;
        }

        // Add totals row if totals are provided
        if (totalNetAmount.HasValue || totalVatAmount.HasValue || totalTtcAmount.HasValue)
        {
            var totalsRow = rowIndex;
            colIndex = 1;
            var formatString = decimalPlaces == 3 ? "#,##0.000" : $"#,##0.{new string('0', decimalPlaces)}";
            
            foreach (var column in columns)
            {
                var cell = worksheet.Cells[totalsRow, colIndex];
                
                if (column.PropertyName.Equals("NetAmount", StringComparison.OrdinalIgnoreCase) && totalNetAmount.HasValue)
                {
                    cell.Value = totalNetAmount.Value;
                    cell.Style.Numberformat.Format = formatString;
                    cell.Style.Font.Bold = true;
                }
                else if (column.PropertyName.Equals("VatAmount", StringComparison.OrdinalIgnoreCase) && totalVatAmount.HasValue)
                {
                    cell.Value = totalVatAmount.Value;
                    cell.Style.Numberformat.Format = formatString;
                    cell.Style.Font.Bold = true;
                }
                else if (colIndex == 1)
                {
                    // First column: "Total" label
                    cell.Value = "Total";
                    cell.Style.Font.Bold = true;
                }
                else
                {
                    cell.Value = string.Empty;
                }
                
                cell.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                cell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                colIndex++;
            }
            
            // Add TTC column value in totals row if needed
            if (addTtcColumn && totalTtcAmount.HasValue)
            {
                var ttcColIndex = columns.Count + 1;
                var ttcCell = worksheet.Cells[totalsRow, ttcColIndex];
                ttcCell.Value = totalTtcAmount.Value;
                ttcCell.Style.Numberformat.Format = formatString;
                ttcCell.Style.Font.Bold = true;
                ttcCell.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                ttcCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                ttcCell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            }
        }

        // Auto-fit columns
        worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

        return package.GetAsByteArray();
    }

    private object? GetPropertyValue(object obj, string propertyName)
    {
        var property = obj.GetType().GetProperty(propertyName);
        if (property == null)
        {
            // Try with different casing
            property = obj.GetType().GetProperty(propertyName, 
                System.Reflection.BindingFlags.IgnoreCase | 
                System.Reflection.BindingFlags.Public | 
                System.Reflection.BindingFlags.Instance);
        }
        
        return property?.GetValue(obj);
    }
}

public class ColumnMapping
{
    public string PropertyName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
}

