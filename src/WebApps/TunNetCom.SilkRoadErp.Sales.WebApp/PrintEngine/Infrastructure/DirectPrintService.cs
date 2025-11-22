using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

namespace TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Infrastructure;

public class DirectPrintService : IPrintService
{
    private readonly IPrinterService _printerService;
    private readonly IJSRuntime _jsRuntime;
    private readonly ILogger<DirectPrintService> _logger;
    private readonly IConfiguration _configuration;
    private readonly ILoggerFactory _loggerFactory;

    public DirectPrintService(
        IPrinterService printerService,
        IJSRuntime jsRuntime,
        ILogger<DirectPrintService> logger,
        IConfiguration configuration,
        ILoggerFactory loggerFactory)
    {
        _printerService = printerService;
        _jsRuntime = jsRuntime;
        _logger = logger;
        _configuration = configuration;
        _loggerFactory = loggerFactory;
    }

    public async Task<PrintResult> PrintAsync(byte[] pdfBytes, PrintOptions options, CancellationToken cancellationToken = default)
    {
        if (options.Mode == PrintMode.Download)
        {
            // Fallback to PDF download
            var pdfService = new PrintToPdfService(_jsRuntime, 
                _loggerFactory.CreateLogger<PrintToPdfService>());
            return await pdfService.PrintAsync(pdfBytes, options, cancellationToken);
        }

        if (string.IsNullOrEmpty(options.PrinterName))
        {
            return PrintResult.Failure("Printer name is required for direct printing");
        }

        try
        {
            var printJobOptions = new PrintJobOptions
            {
                Copies = options.Copies,
                Duplex = options.Duplex
            };

            var result = await _printerService.PrintAsync(
                pdfBytes,
                options.PrinterName,
                printJobOptions,
                cancellationToken);

            if (result.IsSuccess)
            {
                _logger.LogInformation("Print job submitted successfully to printer: {PrinterName}", options.PrinterName);
            }
            else
            {
                _logger.LogError("Print job failed: {ErrorMessage}", result.ErrorMessage);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in direct print service");
            return PrintResult.Failure($"Failed to print: {ex.Message}");
        }
    }
}
