using Microsoft.JSInterop;

namespace TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Infrastructure;

public class DirectPrintService : IPrintService
{
    private readonly IPrinterService _printerService;
    private readonly IJSRuntime _jsRuntime;
    private readonly ILogger<DirectPrintService> _logger;
    private readonly IConfiguration _configuration;
    private readonly ILoggerFactory _loggerFactory;
    private BrowserPrinterService? _browserPrinterService;

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

        // Check if remote printing service is configured
        var serviceUrl = _configuration["Printing:ServiceUrl"];
        var useRemoteService = !string.IsNullOrEmpty(serviceUrl);

        // If remote service is not configured, use browser printing
        if (!useRemoteService)
        {
            _logger.LogInformation("Remote printing service not configured, using browser-based printing");
            return await PrintViaBrowserAsync(pdfBytes, options, cancellationToken);
        }

        // Try remote service first
        if (string.IsNullOrEmpty(options.PrinterName))
        {
            // If no printer name specified but remote service is available, still try browser printing
            _logger.LogWarning("Printer name not specified, falling back to browser printing");
            return await PrintViaBrowserAsync(pdfBytes, options, cancellationToken);
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
                return result;
            }
            else
            {
                _logger.LogWarning("Remote print job failed: {ErrorMessage}, falling back to browser printing", result.ErrorMessage);
                // Fallback to browser printing if remote service fails
                return await PrintViaBrowserAsync(pdfBytes, options, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in remote print service, falling back to browser printing");
            // Fallback to browser printing on exception
            return await PrintViaBrowserAsync(pdfBytes, options, cancellationToken);
        }
    }

    private async Task<PrintResult> PrintViaBrowserAsync(byte[] pdfBytes, PrintOptions options, CancellationToken cancellationToken)
    {
        try
        {
            // Initialize browser printer service if not already done
            _browserPrinterService ??= new BrowserPrinterService(
                _jsRuntime,
                _loggerFactory.CreateLogger<BrowserPrinterService>());

            // Use "Browser Print Dialog" as printer name for browser printing
            var printerName = string.IsNullOrEmpty(options.PrinterName) 
                ? "Browser Print Dialog" 
                : options.PrinterName;

            var printJobOptions = new PrintJobOptions
            {
                Copies = options.Copies,
                Duplex = options.Duplex
            };

            var result = await _browserPrinterService.PrintAsync(
                pdfBytes,
                printerName,
                printJobOptions,
                cancellationToken);

            if (result.IsSuccess)
            {
                _logger.LogInformation("Browser print dialog opened successfully");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in browser print service");
            return PrintResult.Failure($"Failed to print via browser: {ex.Message}");
        }
    }
}
