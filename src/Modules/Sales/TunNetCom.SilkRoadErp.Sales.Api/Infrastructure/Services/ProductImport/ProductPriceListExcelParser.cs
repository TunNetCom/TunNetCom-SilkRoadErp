using System.Globalization;
using OfficeOpenXml;
using TunNetCom.SilkRoadErp.Sales.Api.Features.Products.Import;
using TunNetCom.SilkRoadErp.Sales.Contracts.Products;

namespace TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services.ProductImport;

public class ProductPriceListExcelParser : IProductPriceListExcelParser
{
    private static readonly CultureInfo FrenchCulture = CultureInfo.GetCultureInfo("fr-FR");
    private const int MaxImportRows = 5000;

    private readonly ILogger<ProductPriceListExcelParser> _logger;

    public ProductPriceListExcelParser(ILogger<ProductPriceListExcelParser> logger)
    {
        _logger = logger;
    }

    public Task<ProductImportPreviewResponse> PreviewAsync(Stream stream, int sheetIndex = 0, int maxPreviewRows = 50, CancellationToken cancellationToken = default)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        var response = new ProductImportPreviewResponse();

        using (var package = new ExcelPackage(stream))
        {
            var worksheets = package.Workbook.Worksheets.ToList();
            response = new ProductImportPreviewResponse
            {
                SheetNames = worksheets.Select(w => w.Name).ToList(),
                SelectedSheetIndex = sheetIndex
            };

            if (sheetIndex < 0 || sheetIndex >= worksheets.Count)
            {
                _logger.LogWarning("Sheet index {SheetIndex} out of range, using 0", sheetIndex);
                sheetIndex = 0;
            }

            var sheet = worksheets[sheetIndex];
            var rowCount = sheet.Dimension?.Rows ?? 0;
            var colCount = sheet.Dimension?.Columns ?? 0;

            if (rowCount < 1 || colCount < 1)
            {
                response.Headers = new List<string>();
                response.Rows = new List<Dictionary<string, object?>>();
                return Task.FromResult(response);
            }

            var headers = new List<string>();
            for (var c = 1; c <= colCount; c++)
            {
                var v = sheet.Cells[1, c].GetValue<string>();
                headers.Add((v ?? "").Trim());
            }
            response.Headers = headers;

            var rows = new List<Dictionary<string, object?>>();
            var dataRowCount = Math.Min(rowCount - 1, maxPreviewRows);
            for (var row = 2; row <= 1 + dataRowCount; row++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var dict = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
                for (var c = 1; c <= colCount; c++)
                {
                    var header = headers[c - 1];
                    if (string.IsNullOrEmpty(header))
                        header = $"Col{c}";
                    var val = sheet.Cells[row, c].Value;
                    dict[header] = val;
                }
                rows.Add(dict);
            }
            response.Rows = rows;
        }

        return Task.FromResult(response);
    }

    public Task<IReadOnlyList<ProductImportRowDto>> ParseWithMappingAsync(Stream stream, ProductImportMappingRequest mapping, CancellationToken cancellationToken = default)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        var list = new List<ProductImportRowDto>();

        using (var package = new ExcelPackage(stream))
        {
            var worksheets = package.Workbook.Worksheets.ToList();
            var sheetIndex = Math.Clamp(mapping.SheetIndex, 0, Math.Max(0, worksheets.Count - 1));
            var sheet = worksheets[sheetIndex];
            var rowCount = sheet.Dimension?.Rows ?? 0;
            var colCount = sheet.Dimension?.Columns ?? 0;

            if (rowCount < 2 || colCount < 1)
            {
                return Task.FromResult<IReadOnlyList<ProductImportRowDto>>(list);
            }

            var headerToCol = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            for (var c = 1; c <= colCount; c++)
            {
                var v = sheet.Cells[1, c].GetValue<string>();
                var h = (v ?? "").Trim();
                if (!string.IsNullOrEmpty(h))
                    headerToCol[h] = c;
            }

            if (!headerToCol.TryGetValue(mapping.ReferenceColumn, out var refCol) ||
                !headerToCol.TryGetValue(mapping.PrixBrutColumn, out var prixCol))
            {
                _logger.LogWarning("Reference column '{Ref}' or PrixBrut column '{Prix}' not found in sheet",
                    mapping.ReferenceColumn, mapping.PrixBrutColumn);
                return Task.FromResult<IReadOnlyList<ProductImportRowDto>>(list);
            }

            var nameCol = mapping.NameColumn != null && headerToCol.TryGetValue(mapping.NameColumn, out var nc) ? nc : (int?)null;
            var societeCol = mapping.SocieteColumn != null && headerToCol.TryGetValue(mapping.SocieteColumn, out var sc) ? sc : (int?)null;

            var maxRows = Math.Min(rowCount - 1, MaxImportRows);
            for (var row = 2; row <= 1 + maxRows; row++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var reference = GetCellString(sheet, row, refCol);
                if (string.IsNullOrWhiteSpace(reference))
                    continue;

                var prixBrut = GetCellDecimal(sheet, row, prixCol);
                var name = nameCol.HasValue ? GetCellString(sheet, row, nameCol.Value) : null;
                var societe = societeCol.HasValue ? GetCellString(sheet, row, societeCol.Value) : null;

                list.Add(new ProductImportRowDto
                {
                    Reference = reference.Trim(),
                    Name = string.IsNullOrWhiteSpace(name) ? null : name.Trim(),
                    PrixBrut = prixBrut,
                    Societe = string.IsNullOrWhiteSpace(societe) ? null : societe.Trim()
                });
            }
        }

        return Task.FromResult<IReadOnlyList<ProductImportRowDto>>(list);
    }

    private static string GetCellString(ExcelWorksheet sheet, int row, int col1Based)
    {
        return sheet.Cells[row, col1Based].GetValue<string>()?.Trim() ?? "";
    }

    private static decimal GetCellDecimal(ExcelWorksheet sheet, int row, int col1Based)
    {
        var v = sheet.Cells[row, col1Based].Value;
        if (v is decimal dec)
            return dec;
        if (v is double d)
            return (decimal)d;
        var s = v?.ToString()?.Trim();
        if (string.IsNullOrEmpty(s))
            return 0;
        s = s.Replace(",", ".");
        if (decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var parsed))
            return parsed;
        s = s.Replace(".", ",");
        if (decimal.TryParse(s, NumberStyles.Any, FrenchCulture, out parsed))
            return parsed;
        return 0;
    }
}
