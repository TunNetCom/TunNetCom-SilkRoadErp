using System.Globalization;
using System.Text;
using OfficeOpenXml;

namespace TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services.BankStatement;

public class BankStatementFileParser : IBankStatementFileParser
{
    private static readonly string[] DateOperationHeaders = { "Date Opération", "Date Operation", "Date operation" };
    private static readonly string[] DateValeurHeaders = { "Date valeur", "Date value" };
    private static readonly string[] OperationHeaders = { "Opération", "Operation" };
    private static readonly string[] ReferenceHeaders = { "Référence", "Reference" };
    private static readonly string[] DebitHeaders = { "Débit", "Debit" };
    private static readonly string[] CreditHeaders = { "Crédit", "Credit" };

    private static readonly CultureInfo FrenchCulture = CultureInfo.GetCultureInfo("fr-FR");

    private readonly ILogger<BankStatementFileParser> _logger;

    public BankStatementFileParser(ILogger<BankStatementFileParser> logger)
    {
        _logger = logger;
    }

    public async Task<IReadOnlyList<BankStatementRowDto>> ParseAsync(Stream stream, string? fileName, CancellationToken cancellationToken = default)
    {
        var extension = fileName != null ? Path.GetExtension(fileName).ToLowerInvariant() : "";
        if (extension == ".xlsx" || extension == ".xls")
        {
            return await ParseExcelAsync(stream, cancellationToken).ConfigureAwait(false);
        }
        if (extension == ".csv" || extension == ".txt" || extension == "")
        {
            return await ParseTsvOrCsvAsync(stream, cancellationToken).ConfigureAwait(false);
        }
        _logger.LogWarning("Unknown file extension {Extension}, trying TSV parsing", extension);
        return await ParseTsvOrCsvAsync(stream, cancellationToken).ConfigureAwait(false);
    }

    private async Task<IReadOnlyList<BankStatementRowDto>> ParseExcelAsync(Stream stream, CancellationToken cancellationToken)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        var list = new List<BankStatementRowDto>();
        using (var package = new ExcelPackage(stream))
        {
            var sheet = package.Workbook.Worksheets.FirstOrDefault();
            if (sheet == null)
            {
                return list;
            }
            var rowCount = sheet.Dimension?.Rows ?? 0;
            if (rowCount < 2)
            {
                return list;
            }
            var colCount = sheet.Dimension?.Columns ?? 0;
            var headerRow = 1;
            var colMap = BuildColumnMap(sheet, headerRow, colCount);
            if (colMap == null)
            {
                _logger.LogWarning("Could not find expected headers in Excel");
                return list;
            }
            for (var row = 2; row <= rowCount; row++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var dto = ReadRow(sheet, row, colMap);
                if (dto != null)
                {
                    list.Add(dto);
                }
            }
        }
        await Task.CompletedTask.ConfigureAwait(false);
        return list;
    }

    private async Task<IReadOnlyList<BankStatementRowDto>> ParseTsvOrCsvAsync(Stream stream, CancellationToken cancellationToken)
    {
        using var reader = new StreamReader(stream, Encoding.UTF8);
        var lines = new List<string>();
        while (await reader.ReadLineAsync(cancellationToken).ConfigureAwait(false) is { } line)
        {
            lines.Add(line);
        }
        if (lines.Count < 2)
        {
            return Array.Empty<BankStatementRowDto>();
        }
        var separator = DetectSeparator(lines[0]);
        var headerCells = SplitLine(lines[0], separator);
        var colMap = BuildColumnMapFromHeaders(headerCells);
        if (colMap == null)
        {
            _logger.LogWarning("Could not find expected headers in TSV/CSV");
            return Array.Empty<BankStatementRowDto>();
        }
        var list = new List<BankStatementRowDto>();
        for (var i = 1; i < lines.Count; i++)
        {
            var cells = SplitLine(lines[i], separator);
            var dto = ReadRowFromCells(cells, colMap);
            if (dto != null)
            {
                list.Add(dto);
            }
        }
        return list;
    }

    private static char DetectSeparator(string firstLine)
    {
        if (firstLine.Contains('\t'))
        {
            return '\t';
        }
        return ';';
    }

    private static string[] SplitLine(string line, char separator)
    {
        return line.Split(separator).Select(s => s.Trim()).ToArray();
    }

    private Dictionary<string, int>? BuildColumnMap(ExcelWorksheet sheet, int headerRow, int colCount)
    {
        var headers = new List<string>();
        for (var c = 1; c <= colCount; c++)
        {
            var v = sheet.Cells[headerRow, c].GetValue<string>();
            headers.Add((v ?? "").Trim());
        }
        return BuildColumnMapFromHeaders(headers);
    }

    private static Dictionary<string, int>? BuildColumnMapFromHeaders(IList<string> headers)
    {
        var map = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        for (var i = 0; i < headers.Count; i++)
        {
            var h = headers[i] ?? "";
            if (DateOperationHeaders.Any(x => string.Equals(h, x, StringComparison.OrdinalIgnoreCase)))
            {
                map["DateOperation"] = i;
            }
            else if (DateValeurHeaders.Any(x => string.Equals(h, x, StringComparison.OrdinalIgnoreCase)))
            {
                map["DateValeur"] = i;
            }
            else if (OperationHeaders.Any(x => string.Equals(h, x, StringComparison.OrdinalIgnoreCase)))
            {
                map["Operation"] = i;
            }
            else if (ReferenceHeaders.Any(x => string.Equals(h, x, StringComparison.OrdinalIgnoreCase)))
            {
                map["Reference"] = i;
            }
            else if (DebitHeaders.Any(x => string.Equals(h, x, StringComparison.OrdinalIgnoreCase)))
            {
                map["Debit"] = i;
            }
            else if (CreditHeaders.Any(x => string.Equals(h, x, StringComparison.OrdinalIgnoreCase)))
            {
                map["Credit"] = i;
            }
        }
        if (map.Count >= 6)
        {
            return map;
        }
        return null;
    }

    private static BankStatementRowDto? ReadRow(ExcelWorksheet sheet, int row, Dictionary<string, int> colMap)
    {
        var dateOp = GetCellDate(sheet, row, colMap, "DateOperation");
        var dateVal = GetCellDate(sheet, row, colMap, "DateValeur");
        var operation = GetCellString(sheet, row, colMap, "Operation");
        var reference = GetCellString(sheet, row, colMap, "Reference");
        var debit = GetCellDecimal(sheet, row, colMap, "Debit");
        var credit = GetCellDecimal(sheet, row, colMap, "Credit");
        if (string.IsNullOrWhiteSpace(operation) && debit == 0 && credit == 0)
        {
            return null;
        }
        return new BankStatementRowDto
        {
            DateOperation = dateOp ?? DateTime.Today,
            DateValeur = dateVal ?? dateOp ?? DateTime.Today,
            Operation = operation ?? "",
            Reference = reference ?? "",
            Debit = debit,
            Credit = credit
        };
    }

    private static DateTime? GetCellDate(ExcelWorksheet sheet, int row, Dictionary<string, int> colMap, string key)
    {
        if (!colMap.TryGetValue(key, out var col))
        {
            return null;
        }
        var v = sheet.Cells[row, col + 1].Value;
        if (v is DateTime dt)
        {
            return dt;
        }
        if (v is double d)
        {
            return DateTime.FromOADate(d);
        }
        var s = v?.ToString()?.Trim();
        if (string.IsNullOrEmpty(s))
        {
            return null;
        }
        if (DateTime.TryParse(s, FrenchCulture, DateTimeStyles.None, out var parsed))
        {
            return parsed;
        }
        if (DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed))
        {
            return parsed;
        }
        return null;
    }

    private static string? GetCellString(ExcelWorksheet sheet, int row, Dictionary<string, int> colMap, string key)
    {
        if (!colMap.TryGetValue(key, out var col))
        {
            return null;
        }
        return sheet.Cells[row, col + 1].GetValue<string>()?.Trim();
    }

    private static decimal GetCellDecimal(ExcelWorksheet sheet, int row, Dictionary<string, int> colMap, string key)
    {
        if (!colMap.TryGetValue(key, out var col))
        {
            return 0;
        }
        var v = sheet.Cells[row, col + 1].Value;
        if (v is decimal dec)
        {
            return dec;
        }
        if (v is double d)
        {
            return (decimal)d;
        }
        var s = v?.ToString()?.Trim();
        if (string.IsNullOrEmpty(s))
        {
            return 0;
        }
        s = s.Replace(",", ".");
        if (decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var parsed))
        {
            return parsed;
        }
        s = s.Replace(".", ",");
        if (decimal.TryParse(s, NumberStyles.Any, FrenchCulture, out parsed))
        {
            return parsed;
        }
        return 0;
    }

    private static BankStatementRowDto? ReadRowFromCells(string[] cells, Dictionary<string, int> colMap)
    {
        var dateOp = GetCellDate(cells, colMap, "DateOperation");
        var dateVal = GetCellDate(cells, colMap, "DateValeur");
        var operation = GetCellString(cells, colMap, "Operation");
        var reference = GetCellString(cells, colMap, "Reference");
        var debit = GetCellDecimal(cells, colMap, "Debit");
        var credit = GetCellDecimal(cells, colMap, "Credit");
        if (string.IsNullOrWhiteSpace(operation) && debit == 0 && credit == 0)
        {
            return null;
        }
        return new BankStatementRowDto
        {
            DateOperation = dateOp ?? DateTime.Today,
            DateValeur = dateVal ?? dateOp ?? DateTime.Today,
            Operation = operation ?? "",
            Reference = reference ?? "",
            Debit = debit,
            Credit = credit
        };
    }

    private static DateTime? GetCellDate(string[] cells, Dictionary<string, int> colMap, string key)
    {
        if (!colMap.TryGetValue(key, out var col) || col >= cells.Length)
        {
            return null;
        }
        var s = cells[col].Trim();
        if (string.IsNullOrEmpty(s))
        {
            return null;
        }
        if (DateTime.TryParse(s, FrenchCulture, DateTimeStyles.None, out var parsed))
        {
            return parsed;
        }
        if (DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed))
        {
            return parsed;
        }
        return null;
    }

    private static string? GetCellString(string[] cells, Dictionary<string, int> colMap, string key)
    {
        if (!colMap.TryGetValue(key, out var col) || col >= cells.Length)
        {
            return null;
        }
        return cells[col].Trim();
    }

    private static decimal GetCellDecimal(string[] cells, Dictionary<string, int> colMap, string key)
    {
        if (!colMap.TryGetValue(key, out var col) || col >= cells.Length)
        {
            return 0;
        }
        var s = (cells[col] ?? "").Trim();
        if (string.IsNullOrEmpty(s))
        {
            return 0;
        }
        if (decimal.TryParse(s.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out var parsed))
        {
            return parsed;
        }
        if (decimal.TryParse(s, NumberStyles.Any, FrenchCulture, out parsed))
        {
            return parsed;
        }
        return 0;
    }
}
