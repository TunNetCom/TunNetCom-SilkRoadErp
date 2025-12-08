using Microsoft.JSInterop;

namespace TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Infrastructure;

/// <summary>
/// Browser-based printer service that uses JavaScript to print PDFs via the browser's print dialog.
/// This allows users to select their physical printer (HP, Epson, etc.) from the browser's print dialog.
/// </summary>
public class BrowserPrinterService : IPrinterService
{
    private readonly IJSRuntime _jsRuntime;
    private readonly ILogger<BrowserPrinterService> _logger;

    public BrowserPrinterService(
        IJSRuntime jsRuntime,
        ILogger<BrowserPrinterService> logger)
    {
        _jsRuntime = jsRuntime;
        _logger = logger;
    }

    public async Task<List<PrinterInfo>> GetAvailablePrintersAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            // Try to get printers using browser APIs
            // Chrome/Edge may support navigator.print.getPrinters()
            try
            {
                var printerData = await _jsRuntime.InvokeAsync<List<Dictionary<string, object>>>("getAvailablePrinters");
                
                if (printerData != null && printerData.Any())
                {
                    return printerData.Select(p => new PrinterInfo
                    {
                        Name = p.ContainsKey("name") ? p["name"]?.ToString() ?? "Unknown" : "Unknown",
                        Status = p.ContainsKey("status") ? p["status"]?.ToString() ?? "Ready" : "Ready",
                        IsDefault = p.ContainsKey("isDefault") && p["isDefault"] is bool isDef && isDef,
                        Location = p.ContainsKey("location") ? p["location"]?.ToString() : null
                    }).ToList();
                }
            }
            catch (JSException ex)
            {
                // Browser doesn't support printer enumeration, fall through to default
                _logger.LogDebug(ex, "Browser does not support printer enumeration API");
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Error getting printers from browser API");
            }

            // Fallback: Return a generic option that indicates browser print dialog will be used
            // The browser print dialog will show all available printers including Microsoft Print to PDF
            return new List<PrinterInfo>
            {
                new PrinterInfo
                {
                    Name = "Browser Print Dialog (All Printers)",
                    Status = "Ready",
                    IsDefault = true,
                    Location = "Select printer from browser dialog (HP, Epson, Microsoft Print to PDF, etc.)"
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting available printers");
            // Return default option even on error
            return new List<PrinterInfo>
            {
                new PrinterInfo
                {
                    Name = "Browser Print Dialog (All Printers)",
                    Status = "Ready",
                    IsDefault = true,
                    Location = "Select printer from browser dialog"
                }
            };
        }
    }

    public async Task<PrintResult> PrintAsync(byte[] pdfBytes, string printerName, PrintJobOptions options, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Printing PDF via browser print dialog. Printer: {PrinterName}, Copies: {Copies}, Duplex: {Duplex}", 
                printerName, options.Copies, options.Duplex);

            // Convert PDF bytes to base64 for JavaScript
            var base64Pdf = Convert.ToBase64String(pdfBytes);

            // Call JavaScript function to print the PDF
            // The browser print dialog will open where users can select their printer
            await _jsRuntime.InvokeVoidAsync(
                "printPdfFromBase64",
                base64Pdf,
                options.Copies,
                options.Duplex);

            _logger.LogInformation("Browser print dialog opened successfully");
            return PrintResult.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error printing PDF via browser");
            return PrintResult.Failure($"Failed to print: {ex.Message}");
        }
    }
}

