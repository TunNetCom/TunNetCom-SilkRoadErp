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
        decimal? totalVat19 = null,
        decimal? totalBase7 = null,
        decimal? totalBase13 = null,
        decimal? totalBase19 = null)
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
                // Label in first column
                var labelCell = worksheet.Cells[rowIndex, 1];
                labelCell.Value = "Total HT:";
                labelCell.Style.Font.Bold = true;
                labelCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                labelCell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(249, 249, 249));
                labelCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                
                // Value in last column (don't merge)
                var valueCell = worksheet.Cells[rowIndex, totalColSpan];
                valueCell.Value = totalNetAmount.Value;
                valueCell.Style.Numberformat.Format = formatString;
                valueCell.Style.Font.Bold = true;
                valueCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                valueCell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(249, 249, 249));
                valueCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                
                // Merge only the label cells (leave value cell separate)
                if (totalColSpan > 1)
                {
                    worksheet.Cells[rowIndex, 1, rowIndex, totalColSpan - 1].Merge = true;
                }
                rowIndex++;
            }
            
            // Récapitulatif TVA section - Always show when we have totals
            if (totalNetAmount.HasValue || totalVatAmount.HasValue || totalTtcAmount.HasValue)
            {
                var headerCell = worksheet.Cells[rowIndex, 1];
                headerCell.Value = "Récapitulatif TVA:";
                headerCell.Style.Font.Bold = true;
                headerCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                headerCell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(230, 230, 230));
                worksheet.Cells[rowIndex, 1, rowIndex, totalColSpan].Merge = true;
                worksheet.Cells[rowIndex, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                rowIndex++;
                
                // Base HT 7% - Always show
                var labelCell7 = worksheet.Cells[rowIndex, 1];
                labelCell7.Value = "Base HT 7%:";
                labelCell7.Style.Fill.PatternType = ExcelFillStyle.Solid;
                labelCell7.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(249, 249, 249));
                labelCell7.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                
                var valueCell7 = worksheet.Cells[rowIndex, totalColSpan];
                valueCell7.Value = totalBase7 ?? 0;
                valueCell7.Style.Numberformat.Format = formatString;
                valueCell7.Style.Fill.PatternType = ExcelFillStyle.Solid;
                valueCell7.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(249, 249, 249));
                valueCell7.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                
                if (totalColSpan > 1)
                {
                    worksheet.Cells[rowIndex, 1, rowIndex, totalColSpan - 1].Merge = true;
                }
                rowIndex++;
                
                // Base HT 19% - Always show
                var labelCell19 = worksheet.Cells[rowIndex, 1];
                labelCell19.Value = "Base HT 19%:";
                labelCell19.Style.Fill.PatternType = ExcelFillStyle.Solid;
                labelCell19.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(249, 249, 249));
                labelCell19.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                
                var valueCell19 = worksheet.Cells[rowIndex, totalColSpan];
                valueCell19.Value = totalBase19 ?? 0;
                valueCell19.Style.Numberformat.Format = formatString;
                valueCell19.Style.Fill.PatternType = ExcelFillStyle.Solid;
                valueCell19.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(249, 249, 249));
                valueCell19.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                
                if (totalColSpan > 1)
                {
                    worksheet.Cells[rowIndex, 1, rowIndex, totalColSpan - 1].Merge = true;
                }
                rowIndex++;
                
                // Montant TVA 7% - Always show
                var labelCellVat7 = worksheet.Cells[rowIndex, 1];
                labelCellVat7.Value = "Montant TVA 7%:";
                labelCellVat7.Style.Fill.PatternType = ExcelFillStyle.Solid;
                labelCellVat7.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(249, 249, 249));
                labelCellVat7.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                
                var valueCellVat7 = worksheet.Cells[rowIndex, totalColSpan];
                valueCellVat7.Value = totalVat7 ?? 0;
                valueCellVat7.Style.Numberformat.Format = formatString;
                valueCellVat7.Style.Fill.PatternType = ExcelFillStyle.Solid;
                valueCellVat7.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(249, 249, 249));
                valueCellVat7.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                
                if (totalColSpan > 1)
                {
                    worksheet.Cells[rowIndex, 1, rowIndex, totalColSpan - 1].Merge = true;
                }
                rowIndex++;
                
                // Montant TVA 19% - Always show
                var labelCellVat19 = worksheet.Cells[rowIndex, 1];
                labelCellVat19.Value = "Montant TVA 19%:";
                labelCellVat19.Style.Fill.PatternType = ExcelFillStyle.Solid;
                labelCellVat19.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(249, 249, 249));
                labelCellVat19.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                
                var valueCellVat19 = worksheet.Cells[rowIndex, totalColSpan];
                valueCellVat19.Value = totalVat19 ?? 0;
                valueCellVat19.Style.Numberformat.Format = formatString;
                valueCellVat19.Style.Fill.PatternType = ExcelFillStyle.Solid;
                valueCellVat19.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(249, 249, 249));
                valueCellVat19.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                
                if (totalColSpan > 1)
                {
                    worksheet.Cells[rowIndex, 1, rowIndex, totalColSpan - 1].Merge = true;
                }
                rowIndex++;
            }
            
            // Total TVA row
            if (totalVatAmount.HasValue)
            {
                var labelCell = worksheet.Cells[rowIndex, 1];
                labelCell.Value = "Total TVA:";
                labelCell.Style.Font.Bold = true;
                labelCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                labelCell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(249, 249, 249));
                labelCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                
                var valueCell = worksheet.Cells[rowIndex, totalColSpan];
                valueCell.Value = totalVatAmount.Value;
                valueCell.Style.Numberformat.Format = formatString;
                valueCell.Style.Font.Bold = true;
                valueCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                valueCell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(249, 249, 249));
                valueCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                
                if (totalColSpan > 1)
                {
                    worksheet.Cells[rowIndex, 1, rowIndex, totalColSpan - 1].Merge = true;
                }
                rowIndex++;
            }
            
            // Total TTC row
            if (totalTtcAmount.HasValue)
            {
                var labelCell = worksheet.Cells[rowIndex, 1];
                labelCell.Value = "Total TTC:";
                labelCell.Style.Font.Bold = true;
                labelCell.Style.Font.Size = 12;
                labelCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                labelCell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                labelCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                
                var valueCell = worksheet.Cells[rowIndex, totalColSpan];
                valueCell.Value = totalTtcAmount.Value;
                valueCell.Style.Numberformat.Format = formatString;
                valueCell.Style.Font.Bold = true;
                valueCell.Style.Font.Size = 12;
                valueCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                valueCell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                valueCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                
                if (totalColSpan > 1)
                {
                    worksheet.Cells[rowIndex, 1, rowIndex, totalColSpan - 1].Merge = true;
                }
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

