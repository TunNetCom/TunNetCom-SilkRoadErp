using TunNetCom.SilkRoadErp.Sales.Api.Features.ReceiptNote.CreateReceiptNote;
using TunNetCom.SilkRoadErp.Sales.Contracts.ReceiptNoteLine.Request;
using TunNetCom.SilkRoadErp.Sales.Contracts.ReceiptNoteLine.Response;
using TunNetCom.SilkRoadErp.Sales.Contracts.ReceiptNote.Responses;

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
    public async Task<Result<ReceiptNotesWithSummaryResponse>> GetReceiptNoteWithSummaries(
            int? providerId,
            bool? IsInvoiced,
            int? InvoiceId,
            QueryStringParameters queryParameters,
            CancellationToken cancellationToken)
    {

        var queryParams = new Dictionary<string, string?>
        {
            { "PageNumber", queryParameters.PageNumber.ToString() },
            { "PageSize", queryParameters.PageSize.ToString() },
            { "ProviderId", providerId?.ToString() },
            { "IsInvoiced", IsInvoiced?.ToString()?.ToLower() }
        };

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

        var queryString = string.Join("&",
            queryParams.Where(kvp => !string.IsNullOrWhiteSpace(kvp.Value))
            .Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"));

        var requestUri = $"/receipt_note/summaries?{queryString}";

        var response = await _httpClient.GetAsync(requestUri, cancellationToken);

        _ = response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        var result = System.Text.Json.JsonSerializer.Deserialize<ReceiptNotesWithSummaryResponse>(
            content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return Result.Ok(result);
        
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

        _ = response.EnsureSuccessStatusCode();
        return true;
        //TODO : Handle the response as needed
    }

    public async Task<bool> DetachReceiptNotesFromInvoiceAsync(
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

        if (response.IsSuccessStatusCode)
            return true;
        return false;
    }

    public async Task<Result<ReceiptNotesResponse>> GetReceiptNotes(int PageNumber,
        string SearchKeyword,
        int PageSize,
        string SortProprety,
        string SortOrder,
        CancellationToken cancellationToken)
    {
        var queryParams = new Dictionary<string, string>
        {
            { "PageNumber", PageNumber.ToString() },
            { "PageSize", PageSize.ToString() }
        };

        // Add optional parameters if they are provided
        if (!string.IsNullOrEmpty(SortProprety) && !string.IsNullOrEmpty(SortOrder))
        {
            queryParams.Add("SortProprety", SortProprety);
            queryParams.Add("SortOrder", SortOrder);
        }
        if (!string.IsNullOrEmpty(SearchKeyword))
        {
            queryParams.Add("SearchKeyword", SearchKeyword);
        }

        var queryString = string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"));
        var requestUri = $"/receiptnotes?{queryString}";

        var response = await _httpClient.GetAsync(requestUri, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return Result.Fail("cannot get recipt notes");
        }

        ReceiptNotesResponse receiptNotesResponse = await response.Content.ReadFromJsonAsync<ReceiptNotesResponse>();

        return Result.Ok(receiptNotesResponse);
    }

    public async Task<Result<long>> CreateReceiptNote(CreateReceiptNoteRequest CreateReceiptNoteLineRequest, CancellationToken cancellationToken = default)
    {

        var response = await _httpClient.PostAsJsonAsync("/receiptnotes", CreateReceiptNoteLineRequest, cancellationToken);

        if (!response.IsSuccessStatusCode) 
        { 
            return Result.Fail("failed_to_create_receipt_note");
        }

        var receipId = await response.Content.ReadFromJsonAsync<long>(cancellationToken: cancellationToken);

        return Result.Ok(receipId);
    }

    public async Task<Result<List<int>>> CreateReceiptNoteLines(List<CreateReceiptNoteLineRequest> request, CancellationToken cancellationToken = default)
    {
        var response =await _httpClient.PostAsJsonAsync("/receipt_note_lines", request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return Result.Fail("failed_to_create_receipt_note_line");
        }

        var receiptLineIds = await response.Content.ReadFromJsonAsync<List<int>>(cancellationToken: cancellationToken);

        return Result.Ok(receiptLineIds);
    }

    public async Task<Result<ReceiptNoteResponse>> GetReceiptNoteById(int id, CancellationToken cancellationToken = default)
    {
        var response = await  _httpClient.GetAsync($"/receiptnotes/{id}", cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return Result.Fail("receipt_note_not_found");
        }
        var receiptNote = await response.Content.ReadFromJsonAsync<ReceiptNoteResponse>(cancellationToken: cancellationToken);

        return Result.Ok(receiptNote);
    }

    public async Task<Result<GetReceiptNoteLinesByReceiptNoteIdResponse>> GetReceiptNoteLines(int id, GetReceiptNoteLinesWithSummariesQueryParams queryParams, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"/receipt_note/lines/{id}{queryParams.GetQuery()}", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return Result.Fail("receipt_note_not_found");
        }
        var receiptNote = await response.Content.ReadFromJsonAsync<GetReceiptNoteLinesByReceiptNoteIdResponse>(cancellationToken: cancellationToken);

        return Result.Ok(receiptNote);
    }

    public async Task<Result<int>> CreateReceiptNoteWithLinesRequestTemplate(CreateReceiptNoteWithLinesRequest createReceiptNoteWithLinesRequest, CancellationToken cancellationToken = default)
    {
        var response =await _httpClient.PostAsJsonAsync("/receipt_note_with_lines", createReceiptNoteWithLinesRequest, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to create receipt note with lines. Status Code: {StatusCode}, Reason: {ReasonPhrase}",
                response.StatusCode, response.ReasonPhrase);

            return Result.Fail<int>("failed_to_create_receipt_note_with_lines");
        }
        var receiptId = await response.Content.ReadFromJsonAsync<int>(cancellationToken: cancellationToken);

        return Result.Ok(receiptId);
    }

    public async Task<PagedList<ReceiptNoteDetailResponse>> GetReceiptNotesByProductReferenceAsync(
        string productReference,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var queryParams = $"?PageNumber={pageNumber}&PageSize={pageSize}";
            var response = await _httpClient.GetAsync($"/receiptNoteHistory/{Uri.EscapeDataString(productReference)}{queryParams}", cancellationToken);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return new PagedList<ReceiptNoteDetailResponse>();
            }

            _ = response.EnsureSuccessStatusCode();

            var receiptNotes = await response.Content.ReadFromJsonAsync<PagedList<ReceiptNoteDetailResponse>>(cancellationToken: cancellationToken);

            return receiptNotes ?? new PagedList<ReceiptNoteDetailResponse>();
        }
        catch (HttpRequestException)
        {
            // Handle API errors (e.g., network issues, 500 errors)
            return new PagedList<ReceiptNoteDetailResponse>();
        }
    }
}