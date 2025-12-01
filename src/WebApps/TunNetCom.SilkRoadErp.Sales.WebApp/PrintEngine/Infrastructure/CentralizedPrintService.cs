using TunNetCom.SilkRoadErp.Sales.Domain.Entites;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.PrintHistory;
using TunNetCom.SilkRoadErp.Sales.Contracts.PrintHistory;

namespace TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Infrastructure;

/// <summary>
/// Centralized print service that wraps existing print services and logs all print operations
/// </summary>
public class CentralizedPrintService : IPrintService
{
    private readonly IPrintService _innerPrintService;
    private readonly IPrintHistoryClient _printHistoryClient;
    private readonly ILogger<CentralizedPrintService> _logger;

    public CentralizedPrintService(
        IPrintService innerPrintService,
        IPrintHistoryClient printHistoryClient,
        ILogger<CentralizedPrintService> logger)
    {
        _innerPrintService = innerPrintService;
        _printHistoryClient = printHistoryClient;
        _logger = logger;
    }

    public async Task<PrintResult> PrintAsync(byte[] pdfBytes, PrintOptions options, CancellationToken cancellationToken = default)
    {
        // If document information is provided, log the print operation
        if (!string.IsNullOrEmpty(options.DocumentType) && options.DocumentId.HasValue)
        {
            // Check if document is saved (DocumentId > 0)
            if (options.DocumentId.Value <= 0)
            {
                _logger.LogWarning("Print blocked: Document {DocumentType} with ID {DocumentId} is not saved", 
                    options.DocumentType, options.DocumentId.Value);
                return PrintResult.Failure("Document must be saved before printing");
            }

            // Check if it's a duplicata for customer invoices
            bool isDuplicata = false;
            if (options.DocumentType == "Facture")
            {
                try
                {
                    var existingHistory = await _printHistoryClient.GetPrintHistoryByDocumentAsync(
                        options.DocumentType,
                        options.DocumentId.Value,
                        cancellationToken);
                    
                    isDuplicata = existingHistory.Any();
                    _logger.LogInformation("Invoice {DocumentId} isDuplicata={IsDuplicata} (existing prints: {Count})", 
                        options.DocumentId.Value, isDuplicata, existingHistory.Count);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error checking print history for duplicata determination");
                    // Continue without duplicata check if there's an error
                }
            }

            // Execute the actual print
            var printResult = await _innerPrintService.PrintAsync(pdfBytes, options, cancellationToken);

            // Log the print operation (fire and forget - don't block on logging)
            _ = Task.Run(async () =>
            {
                try
                {
                    var printModeEnum = options.Mode == PrintMode.Download 
                        ? PrintModeEnum.Download 
                        : PrintModeEnum.DirectPrint;

                    var createRequest = new CreatePrintHistoryRequest
                    {
                        DocumentType = options.DocumentType,
                        DocumentId = options.DocumentId.Value,
                        PrintMode = printModeEnum.ToString(),
                        UserId = options.UserId,
                        Username = options.Username,
                        PrinterName = options.PrinterName,
                        Copies = options.Copies,
                        FileName = options.FileName,
                        IsDuplicata = isDuplicata
                    };

                    await _printHistoryClient.CreatePrintHistoryAsync(createRequest, cancellationToken);
                    _logger.LogInformation("Print history logged for {DocumentType} {DocumentId}", 
                        options.DocumentType, options.DocumentId.Value);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error logging print history for {DocumentType} {DocumentId}", 
                        options.DocumentType, options.DocumentId.Value);
                    // Don't fail the print operation if logging fails
                }
            }, cancellationToken);

            return printResult;
        }
        else
        {
            // No document information provided - just pass through to inner service
            // This allows backward compatibility with existing code that doesn't provide document info
            _logger.LogWarning("Print operation without document tracking information");
            return await _innerPrintService.PrintAsync(pdfBytes, options, cancellationToken);
        }
    }
}
