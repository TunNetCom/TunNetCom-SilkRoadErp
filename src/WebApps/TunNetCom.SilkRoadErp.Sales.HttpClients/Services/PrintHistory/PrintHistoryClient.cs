using System.Web;
using TunNetCom.SilkRoadErp.Sales.Contracts.PrintHistory;

namespace TunNetCom.SilkRoadErp.Sales.HttpClients.Services.PrintHistory;

public class PrintHistoryClient : IPrintHistoryClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<PrintHistoryClient> _logger;

    public PrintHistoryClient(HttpClient httpClient, ILogger<PrintHistoryClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<PagedList<PrintHistoryResponse>> GetPrintHistoryAsync(
        GetPrintHistoryRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            // Build query string
            var queryString = $"?pageNumber={request.PageNumber}&pageSize={request.PageSize}";

            if (!string.IsNullOrEmpty(request.DocumentType))
            {
                queryString += $"&documentType={Uri.EscapeDataString(request.DocumentType)}";
            }

            if (request.DocumentId.HasValue)
            {
                queryString += $"&documentId={request.DocumentId.Value}";
            }

            if (request.DateFrom.HasValue)
            {
                queryString += $"&dateFrom={Uri.EscapeDataString(request.DateFrom.Value.ToString("o"))}";
            }

            if (request.DateTo.HasValue)
            {
                queryString += $"&dateTo={Uri.EscapeDataString(request.DateTo.Value.ToString("o"))}";
            }

            if (request.UserId.HasValue)
            {
                queryString += $"&userId={request.UserId.Value}";
            }

            if (!string.IsNullOrEmpty(request.PrintMode))
            {
                queryString += $"&printMode={Uri.EscapeDataString(request.PrintMode)}";
            }

            var response = await _httpClient.GetAsync($"/api/print-history{queryString}", cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<PagedList<PrintHistoryResponse>>(cancellationToken: cancellationToken);
                return result ?? new PagedList<PrintHistoryResponse>();
            }

            throw new Exception($"Failed to fetch print history. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching print history");
            throw;
        }
    }

    public async Task<List<PrintHistoryResponse>> GetPrintHistoryByDocumentAsync(
        string documentType,
        int documentId,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/print-history/{Uri.EscapeDataString(documentType)}/{documentId}", cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<List<PrintHistoryResponse>>(cancellationToken: cancellationToken);
                return result ?? new List<PrintHistoryResponse>();
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return new List<PrintHistoryResponse>();
            }

            throw new Exception($"Failed to fetch print history for document. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching print history for document {DocumentType}:{DocumentId}", documentType, documentId);
            throw;
        }
    }

    public async Task<long> CreatePrintHistoryAsync(
        CreatePrintHistoryRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/print-history", request, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<long>(cancellationToken: cancellationToken);
                return result;
            }

            throw new Exception($"Failed to create print history. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating print history");
            throw;
        }
    }
}