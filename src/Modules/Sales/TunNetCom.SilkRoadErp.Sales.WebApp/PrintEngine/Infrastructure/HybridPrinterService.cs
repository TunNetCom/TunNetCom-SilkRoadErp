namespace TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Infrastructure;

/// <summary>
/// Hybrid printer service that uses RemotePrinterService when available,
/// otherwise falls back to BrowserPrinterService for browser-based printing.
/// </summary>
public class HybridPrinterService : IPrinterService
{
    private readonly IPrinterService _remotePrinterService;
    private readonly BrowserPrinterService _browserPrinterService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<HybridPrinterService> _logger;

    public HybridPrinterService(
        IPrinterService remotePrinterService,
        BrowserPrinterService browserPrinterService,
        IConfiguration configuration,
        ILogger<HybridPrinterService> logger)
    {
        _remotePrinterService = remotePrinterService;
        _browserPrinterService = browserPrinterService;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<List<PrinterInfo>> GetAvailablePrintersAsync(CancellationToken cancellationToken = default)
    {
        var serviceUrl = _configuration["Printing:ServiceUrl"];
        
        // If remote service is configured, try to use it
        if (!string.IsNullOrEmpty(serviceUrl))
        {
            try
            {
                var printers = await _remotePrinterService.GetAvailablePrintersAsync(cancellationToken);
                if (printers != null && printers.Any())
                {
                    _logger.LogDebug("Retrieved {Count} printers from remote service", printers.Count);
                    return printers;
                }
                else
                {
                    _logger.LogWarning("Remote service returned no printers, falling back to browser service");
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error getting printers from remote service, falling back to browser service");
            }
        }
        else
        {
            _logger.LogDebug("Remote printing service not configured, using browser printer service");
        }

        // Fallback to browser printer service
        return await _browserPrinterService.GetAvailablePrintersAsync(cancellationToken);
    }

    public async Task<PrintResult> PrintAsync(byte[] pdfBytes, string printerName, PrintJobOptions options, CancellationToken cancellationToken = default)
    {
        var serviceUrl = _configuration["Printing:ServiceUrl"];
        
        // If remote service is configured, try to use it
        if (!string.IsNullOrEmpty(serviceUrl))
        {
            try
            {
                var result = await _remotePrinterService.PrintAsync(pdfBytes, printerName, options, cancellationToken);
                if (result.IsSuccess)
                {
                    return result;
                }
                else
                {
                    _logger.LogWarning("Remote print failed: {ErrorMessage}, falling back to browser printing", result.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error printing via remote service, falling back to browser printing");
            }
        }

        // Fallback to browser printer service
        return await _browserPrinterService.PrintAsync(pdfBytes, printerName, options, cancellationToken);
    }
}

