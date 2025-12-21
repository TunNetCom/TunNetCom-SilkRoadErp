using Microsoft.Playwright;
using System.Text;

namespace TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services;

public class PdfListExportService
{
    private readonly ILogger<PdfListExportService> _logger;
    private static bool _playwrightInstalled = false;
    private static readonly SemaphoreSlim _installSemaphore = new SemaphoreSlim(1, 1);

    public PdfListExportService(ILogger<PdfListExportService> logger)
    {
        _logger = logger;
    }

    public async Task<byte[]> ExportToPdfAsync<T>(
        IEnumerable<T> data,
        List<ColumnMapping> columns,
        string title = "Export",
        int decimalPlaces = 3,
        decimal? totalNetAmount = null,
        decimal? totalVatAmount = null,
        decimal? totalTtcAmount = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await EnsurePlaywrightInstalledAsync(cancellationToken);

            var html = GenerateHtmlTable(data, columns, title, decimalPlaces, totalNetAmount, totalVatAmount, totalTtcAmount);
            _logger.LogDebug("HTML generated successfully, length: {HtmlLength} characters", html?.Length ?? 0);

            return await GeneratePdfFromHtmlAsync(html, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating PDF export");
            throw;
        }
    }

    private string GenerateHtmlTable<T>(IEnumerable<T> data, List<ColumnMapping> columns, string title, int decimalPlaces = 3, decimal? totalNetAmount = null, decimal? totalVatAmount = null, decimal? totalTtcAmount = null)
    {
        var html = new StringBuilder();
        html.AppendLine("<!DOCTYPE html>");
        html.AppendLine("<html>");
        html.AppendLine("<head>");
        html.AppendLine("<meta charset='utf-8'>");
        html.AppendLine("<style>");
        html.AppendLine("body { font-family: Arial, sans-serif; margin: 20px; }");
        html.AppendLine("h1 { color: #333; margin-bottom: 20px; }");
        html.AppendLine("table { width: 100%; border-collapse: collapse; margin-top: 10px; }");
        html.AppendLine("th, td { border: 1px solid #ddd; padding: 8px; text-align: left; }");
        html.AppendLine("th { background-color: #f2f2f2; font-weight: bold; }");
        html.AppendLine("tr:nth-child(even) { background-color: #f9f9f9; }");
        html.AppendLine("</style>");
        html.AppendLine("</head>");
        html.AppendLine("<body>");
        html.AppendLine($"<h1>{title}</h1>");
        html.AppendLine("<table>");
        
        // Header row
        html.AppendLine("<thead>");
        html.AppendLine("<tr>");
        foreach (var column in columns)
        {
            html.AppendLine($"<th>{System.Net.WebUtility.HtmlEncode(column.DisplayName)}</th>");
        }
        
        // Add TTC column header if NetAmount and VatAmount columns exist
        var hasNetAmount = columns.Any(c => c.PropertyName.Equals("NetAmount", StringComparison.OrdinalIgnoreCase));
        var hasVatAmount = columns.Any(c => c.PropertyName.Equals("VatAmount", StringComparison.OrdinalIgnoreCase));
        if (hasNetAmount && hasVatAmount)
        {
            html.AppendLine("<th>Total TTC</th>");
        }
        
        html.AppendLine("</tr>");
        html.AppendLine("</thead>");
        
        // Data rows
        html.AppendLine("<tbody>");
        foreach (var item in data)
        {
            html.AppendLine("<tr>");
            foreach (var column in columns)
            {
                var value = GetPropertyValue(item, column.PropertyName);
                var displayValue = FormatValue(value, decimalPlaces);
                html.AppendLine($"<td>{System.Net.WebUtility.HtmlEncode(displayValue)}</td>");
            }
            
            // Add TTC column if NetAmount and VatAmount columns exist
            if (hasNetAmount && hasVatAmount)
            {
                var netAmount = GetPropertyValue(item, "NetAmount");
                var vatAmount = GetPropertyValue(item, "VatAmount");
                decimal ttcValue = 0;
                if (netAmount is decimal net && vatAmount is decimal vat)
                {
                    ttcValue = net + vat;
                }
                var formattedTtc = FormatValue(ttcValue, decimalPlaces);
                html.AppendLine($"<td>{System.Net.WebUtility.HtmlEncode(formattedTtc)}</td>");
            }
            
            html.AppendLine("</tr>");
        }
        html.AppendLine("</tbody>");
        
        // Add totals row if totals are provided
        if (totalNetAmount.HasValue || totalVatAmount.HasValue || totalTtcAmount.HasValue)
        {
            html.AppendLine("<tfoot>");
            html.AppendLine("<tr style='background-color: #f2f2f2; font-weight: bold;'>");
            foreach (var column in columns)
            {
                if (column.PropertyName.Equals("NetAmount", StringComparison.OrdinalIgnoreCase) && totalNetAmount.HasValue)
                {
                    var formattedValue = FormatValue(totalNetAmount.Value, decimalPlaces);
                    html.AppendLine($"<td>{System.Net.WebUtility.HtmlEncode(formattedValue)}</td>");
                }
                else if (column.PropertyName.Equals("VatAmount", StringComparison.OrdinalIgnoreCase) && totalVatAmount.HasValue)
                {
                    var formattedValue = FormatValue(totalVatAmount.Value, decimalPlaces);
                    html.AppendLine($"<td>{System.Net.WebUtility.HtmlEncode(formattedValue)}</td>");
                }
                else if (columns.IndexOf(column) == 0)
                {
                    html.AppendLine("<td>Total</td>");
                }
                else
                {
                    html.AppendLine("<td></td>");
                }
            }
            
            // Add TTC column if NetAmount and VatAmount columns exist
            if (hasNetAmount && hasVatAmount && totalTtcAmount.HasValue)
            {
                var formattedTtc = FormatValue(totalTtcAmount.Value, decimalPlaces);
                html.AppendLine($"<td>{System.Net.WebUtility.HtmlEncode(formattedTtc)}</td>");
            }
            
            html.AppendLine("</tr>");
            html.AppendLine("</tfoot>");
        }
        
        html.AppendLine("</table>");
        html.AppendLine("</body>");
        html.AppendLine("</html>");
        
        return html.ToString();
    }

    private string FormatValue(object? value, int decimalPlaces = 3)
    {
        if (value == null)
            return string.Empty;

        var formatString = $"N{decimalPlaces}";
        return value switch
        {
            decimal d => d.ToString(formatString, System.Globalization.CultureInfo.GetCultureInfo("fr-FR")),
            double dbl => dbl.ToString(formatString, System.Globalization.CultureInfo.GetCultureInfo("fr-FR")),
            float f => f.ToString(formatString, System.Globalization.CultureInfo.GetCultureInfo("fr-FR")),
            DateTime dt => dt.ToString("dd/MM/yyyy"),
            DateTimeOffset dto => dto.ToString("dd/MM/yyyy"),
            _ => value.ToString() ?? string.Empty
        };
    }

    private object? GetPropertyValue(object obj, string propertyName)
    {
        var property = obj.GetType().GetProperty(propertyName);
        if (property == null)
        {
            property = obj.GetType().GetProperty(propertyName,
                System.Reflection.BindingFlags.IgnoreCase |
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.Instance);
        }

        return property?.GetValue(obj);
    }

    private async Task EnsurePlaywrightInstalledAsync(CancellationToken cancellationToken)
    {
        if (_playwrightInstalled)
            return;

        await _installSemaphore.WaitAsync(cancellationToken);
        try
        {
            if (_playwrightInstalled)
                return;

            _logger.LogInformation("Installing Playwright browsers...");
            var exitCode = Microsoft.Playwright.Program.Main(new[] { "install", "chromium" });
            if (exitCode != 0)
            {
                throw new Exception($"Playwright install failed with exit code {exitCode}");
            }

            _playwrightInstalled = true;
            _logger.LogInformation("Playwright browsers installed successfully");
        }
        finally
        {
            _installSemaphore.Release();
        }
    }

    private async Task<byte[]> GeneratePdfFromHtmlAsync(string html, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogDebug("Creating Playwright instance...");
            using var playwright = await Playwright.CreateAsync();
            
            _logger.LogDebug("Launching Chromium browser...");
            await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
            
            _logger.LogDebug("Creating new page...");
            var page = await browser.NewPageAsync();
            
            _logger.LogDebug("Setting page content...");
            await page.SetContentAsync(html);

            _logger.LogDebug("Generating PDF...");
            var pdfBytes = await page.PdfAsync(new PagePdfOptions
            {
                Format = "A4",
                PrintBackground = true,
                Margin = new Margin
                {
                    Top = "20mm",
                    Right = "15mm",
                    Bottom = "20mm",
                    Left = "15mm"
                }
            });
            
            _logger.LogInformation("PDF generated successfully, size: {PdfSize} bytes", pdfBytes?.Length ?? 0);
            return pdfBytes;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Playwright error while generating PDF. Message: {Message}", ex.Message);
            throw new InvalidOperationException($"Failed to generate PDF using Playwright: {ex.Message}", ex);
        }
    }
}

