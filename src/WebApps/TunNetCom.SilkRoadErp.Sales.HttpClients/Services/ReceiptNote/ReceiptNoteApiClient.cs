using System.Net.Http;
using System.Text.Json;
using TunNetCom.SilkRoadErp.Sales.Contracts.RecieptNotes;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Invoices;

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
    public async Task<OneOf<PagedList<ReceiptNoteDetailsResponse>, BadRequestResponse>> GetReceiptNote(
      int providerId,
      QueryStringParameters queryParameters,
      CancellationToken cancellationToken)
    {
        var url = $"/api/receipt-notes?pageNumber={queryParameters.PageNumber}" +
                    $"&pageSize={queryParameters.PageSize}" +
                   $"&idFournisseur={providerId}";
        var response = await _httpClient.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        var pagedReceiptNotes = JsonConvert.DeserializeObject<PagedList<ReceiptNoteDetailsResponse>>(responseContent);

        return pagedReceiptNotes;
    }
    public async Task<PagedList<ReceiptNoteDetailsResponse>> GetReceiptNotesAsync(
       int? idFournisseur,
       int pageNumber,
       int pageSize,
       string? searchKeyword,
       bool? isFactured,
       CancellationToken cancellationToken)
    {
        var queryParams = new Dictionary<string, string>
        {
            { "PageNumber", pageNumber.ToString() },
            { "PageSize", pageSize.ToString() }
        };

        if (idFournisseur.HasValue)
            queryParams.Add("IdFournisseur", idFournisseur.Value.ToString());

        if (!string.IsNullOrEmpty(searchKeyword))
            queryParams.Add("SearchKeyword", searchKeyword);

        if (isFactured.HasValue)
            queryParams.Add("IsFactured", isFactured.Value.ToString());

        var queryString = string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"));
        var requestUri = $"/receipt-note?{queryString}";

        // Make the HTTP request
        var response = await _httpClient.GetAsync(requestUri, cancellationToken);
        response.EnsureSuccessStatusCode();

        // Deserialize the response
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        var result = System.Text.Json.JsonSerializer.Deserialize<PagedList<ReceiptNoteDetailsResponse>>(content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return result ?? throw new InvalidOperationException("Failed to deserialize response");
    }
}

   
