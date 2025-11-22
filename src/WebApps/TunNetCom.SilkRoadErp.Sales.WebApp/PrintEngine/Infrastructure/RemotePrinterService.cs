using System.Net.Http.Json;
using System.Text.Json;
using TunNetCom.SilkRoadErp.Sales.Contracts.Printing;

namespace TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Infrastructure;

public class RemotePrinterService : IPrinterService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<RemotePrinterService> _logger;
    private readonly string? _serviceUrl;

    public RemotePrinterService(
        HttpClient httpClient,
        ILogger<RemotePrinterService> logger,
        IConfiguration configuration)
    {
        _httpClient = httpClient;
        _logger = logger;
        _serviceUrl = configuration["Printing:ServiceUrl"];
    }

    public async Task<List<PrinterInfo>> GetAvailablePrintersAsync(CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(_serviceUrl))
        {
            _logger.LogWarning("Printing service URL not configured, returning empty printer list");
            return new List<PrinterInfo>();
        }

        try
        {
            var response = await _httpClient.GetAsync($"{_serviceUrl}/printers", cancellationToken);
            
            if (response.IsSuccessStatusCode)
            {
                var printers = await response.Content.ReadFromJsonAsync<List<PrinterInfoDto>>(
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
                    cancellationToken);
                
                return printers?.Select(p => new PrinterInfo
                {
                    Name = p.Name,
                    Status = p.Status,
                    IsDefault = p.IsDefault,
                    Location = p.Location
                }).ToList() ?? new List<PrinterInfo>();
            }
            else
            {
                _logger.LogError("Failed to get printers. Status: {StatusCode}", response.StatusCode);
                return new List<PrinterInfo>();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting available printers");
            return new List<PrinterInfo>();
        }
    }

    public async Task<PrintResult> PrintAsync(byte[] pdfBytes, string printerName, PrintJobOptions options, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(_serviceUrl))
        {
            return PrintResult.Failure("Printing service URL not configured");
        }

        try
        {
            var request = new PrintJobRequest
            {
                PdfBytes = pdfBytes,
                PrinterName = printerName,
                Copies = options.Copies,
                Duplex = options.Duplex,
                PaperSize = options.PaperSize,
                Orientation = options.Orientation
            };

            var response = await _httpClient.PostAsJsonAsync($"{_serviceUrl}/print", request, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<PrintJobResponse>(
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
                    cancellationToken);

                if (result != null && result.Status == "Success")
                {
                    _logger.LogInformation("Print job submitted successfully. JobId: {JobId}", result.JobId);
                    return PrintResult.Success(result.JobId);
                }
                else
                {
                    var errorMessage = result?.ErrorMessage ?? "Unknown error";
                    _logger.LogError("Print job failed: {ErrorMessage}", errorMessage);
                    return PrintResult.Failure(errorMessage);
                }
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogError("Failed to submit print job. Status: {StatusCode}, Content: {Content}", 
                    response.StatusCode, errorContent);
                return PrintResult.Failure($"Print service returned status {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error submitting print job");
            return PrintResult.Failure($"Failed to submit print job: {ex.Message}");
        }
    }
}

