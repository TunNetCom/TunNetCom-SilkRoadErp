using Microsoft.JSInterop;

namespace TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Infrastructure;

public class PrintToPdfService : IPrintService
{
    private readonly IJSRuntime _jsRuntime;
    private readonly ILogger<PrintToPdfService> _logger;

    public PrintToPdfService(IJSRuntime jsRuntime, ILogger<PrintToPdfService> logger)
    {
        _jsRuntime = jsRuntime;
        _logger = logger;
    }

    public async Task<PrintResult> PrintAsync(byte[] pdfBytes, PrintOptions options, CancellationToken cancellationToken = default)
    {
        try
        {
            var fileName = options.FileName ?? $"document_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
            
            await _jsRuntime.InvokeVoidAsync(
                "downloadFile",
                fileName,
                Convert.ToBase64String(pdfBytes),
                "application/pdf");

            _logger.LogInformation("PDF downloaded successfully: {FileName}", fileName);
            return PrintResult.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to download PDF");
            return PrintResult.Failure($"Failed to download PDF: {ex.Message}");
        }
    }
}

