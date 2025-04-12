using System.Text.Json;
using TunNetCom.SilkRoadErp.Sales.Contracts.RecieptNotes;
namespace TunNetCom.SilkRoadErp.Sales.HttpClients.Services.ReceiptNote;

class ReceiptNoteApiClient : IReceiptNoteApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ReceiptNoteApiClient> _logger;
    public ReceiptNoteApiClient(HttpClient httpClient, ILogger<ReceiptNoteApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }
    public async Task<ReceiptNotesWithSummary> GetReceiptNote(
            int providerId,
            bool IsInvoiced,
            int? InvoiceId,
            QueryStringParameters queryParameters,
            CancellationToken cancellationToken)
    {

        var queryParams = new Dictionary<string, string>
        {
            { "PageNumber", queryParameters.PageNumber.ToString() },
            { "PageSize", queryParameters.PageSize.ToString() },
            { "ProviderId", providerId.ToString() },
            { "IsInvoiced", IsInvoiced.ToString() }
        };

        // Add optional parameters if they are provided
        if (!string.IsNullOrEmpty(queryParameters.SortProprety) && !string.IsNullOrEmpty(queryParameters.SortOrder))
        {
            queryParams.Add("SortProprety", queryParameters.SortProprety);
            queryParams.Add("SortOrder", queryParameters.SortOrder);
        }
        if (!string.IsNullOrEmpty(queryParameters.SearchKeyword))
        {
            queryParams.Add("SearchKeyword", queryParameters.SearchKeyword);
        }
        if (InvoiceId.HasValue)
        {
            queryParams.Add("InvoiceId", InvoiceId.Value.ToString());
        }

        // Build the query string
        var queryString = string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"));
        var requestUri = $"/receipt-note?{queryString}";

        // Send the GET request
        var response = await _httpClient.GetAsync(requestUri, cancellationToken);

        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        var result = System.Text.Json.JsonSerializer.Deserialize<ReceiptNotesWithSummary>(
            content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return result;
        
    }


    public async Task<bool> AttachReceiptNotesToInvoiceAsync(
        int invoiceId,
        List<int> receiptNotesIds,
        CancellationToken cancellationToken = default)
    {
        var command = new AttachReceiptNotesRequest
        { 
            ReceiptNotesIds= receiptNotesIds,
            InvoiceId = invoiceId
        };

        var headers = new Dictionary<string, string>()
        {
        { "Accept", "application/problem+json" }
        };

        var response = await _httpClient.PutAsJsonAsync(
            "/receipt-note/attach-to-invoice",
            command,
            cancellationToken);

        response.EnsureSuccessStatusCode();
        return true;
        //TODO : Handle the response as needed
    }

    public async Task DetachReceiptNotesFromInvoiceAsync(
        int invoiceId,
        List<int> receiptNotesIds,
        CancellationToken cancellationToken = default)
    {
        var command = new DetachReceiptNotesRequest
        {
            ReceiptNoteIds = receiptNotesIds,
            InvoiceId = invoiceId
        };
        var headers = new Dictionary<string, string>()
        {
            { "Accept", "application/problem+json" }
        };
        var response = await _httpClient.PutAsJsonAsync(
            "/receipt-note/detach",
            command,
            cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}

  