using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Text;
using System.Globalization;

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
        decimal? totalTtcAmount = null,
        decimal? totalVat7 = null,
        decimal? totalVat13 = null,
        decimal? totalVat19 = null)
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
                else if (value != null && Nullable.GetUnderlyingType(value.GetType()) == typeof(DateTime))
                {
                    var nullableDate = (DateTime?)value;
                    if (nullableDate.HasValue)
                    {
                        cell.Value = nullableDate.Value;
                        cell.Style.Numberformat.Format = "dd/MM/yyyy";
                    }
                    else
                    {
                        cell.Value = string.Empty;
                    }
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

        // Add totals section if totals are provided
        if (totalNetAmount.HasValue || totalVatAmount.HasValue || totalTtcAmount.HasValue)
        {
            var formatString = decimalPlaces == 3 ? "#,##0.000" : $"#,##0.{new string('0', decimalPlaces)}";
            var totalColSpan = columns.Count + (addTtcColumn ? 1 : 0);
            
            // Total HT row
            if (totalNetAmount.HasValue)
            {
                var cell = worksheet.Cells[rowIndex, 1];
                cell.Value = "Total HT:";
                cell.Style.Font.Bold = true;
                cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                cell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(249, 249, 249));
                
                var valueCell = worksheet.Cells[rowIndex, totalColSpan];
                valueCell.Value = totalNetAmount.Value;
                valueCell.Style.Numberformat.Format = formatString;
                valueCell.Style.Font.Bold = true;
                valueCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                valueCell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(249, 249, 249));
                
                worksheet.Cells[rowIndex, 1, rowIndex, totalColSpan].Merge = true;
                worksheet.Cells[rowIndex, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                rowIndex++;
            }
            
            // TVA detail rows
            if (totalVat7.HasValue && totalVat7.Value > 0)
            {
                var cell = worksheet.Cells[rowIndex, 1];
                cell.Value = $"TVA 7%: {totalVat7.Value.ToString(formatString, CultureInfo.GetCultureInfo("fr-FR"))}";
                cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                cell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(249, 249, 249));
                worksheet.Cells[rowIndex, 1, rowIndex, totalColSpan].Merge = true;
                worksheet.Cells[rowIndex, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                rowIndex++;
            }
            
            if (totalVat13.HasValue && totalVat13.Value > 0)
            {
                var cell = worksheet.Cells[rowIndex, 1];
                cell.Value = $"TVA 13%: {totalVat13.Value.ToString(formatString, CultureInfo.GetCultureInfo("fr-FR"))}";
                cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                cell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(249, 249, 249));
                worksheet.Cells[rowIndex, 1, rowIndex, totalColSpan].Merge = true;
                worksheet.Cells[rowIndex, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                rowIndex++;
            }
            
            if (totalVat19.HasValue && totalVat19.Value > 0)
            {
                var cell = worksheet.Cells[rowIndex, 1];
                cell.Value = $"TVA 19%: {totalVat19.Value.ToString(formatString, CultureInfo.GetCultureInfo("fr-FR"))}";
                cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                cell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(249, 249, 249));
                worksheet.Cells[rowIndex, 1, rowIndex, totalColSpan].Merge = true;
                worksheet.Cells[rowIndex, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                rowIndex++;
            }
            
            // Total TVA row
            if (totalVatAmount.HasValue)
            {
                var cell = worksheet.Cells[rowIndex, 1];
                cell.Value = "Total TVA:";
                cell.Style.Font.Bold = true;
                cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                cell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(249, 249, 249));
                
                var valueCell = worksheet.Cells[rowIndex, totalColSpan];
                valueCell.Value = totalVatAmount.Value;
                valueCell.Style.Numberformat.Format = formatString;
                valueCell.Style.Font.Bold = true;
                valueCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                valueCell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(249, 249, 249));
                
                worksheet.Cells[rowIndex, 1, rowIndex, totalColSpan].Merge = true;
                worksheet.Cells[rowIndex, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                rowIndex++;
            }
            
            // Total TTC row
            if (totalTtcAmount.HasValue)
            {
                var cell = worksheet.Cells[rowIndex, 1];
                cell.Value = "Total TTC:";
                cell.Style.Font.Bold = true;
                cell.Style.Font.Size = 12;
                cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                cell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                
                var valueCell = worksheet.Cells[rowIndex, totalColSpan];
                valueCell.Value = totalTtcAmount.Value;
                valueCell.Style.Numberformat.Format = formatString;
                valueCell.Style.Font.Bold = true;
                valueCell.Style.Font.Size = 12;
                valueCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                valueCell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                
                worksheet.Cells[rowIndex, 1, rowIndex, totalColSpan].Merge = true;
                worksheet.Cells[rowIndex, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
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

