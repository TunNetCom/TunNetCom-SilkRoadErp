namespace TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Invoices;

public class InvoicesApiClient : IInvoicesApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<InvoicesApiClient> _logger;

    public InvoicesApiClient(HttpClient httpClient, ILogger<InvoicesApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }


    public async Task<OneOf<GetInvoiceListWithSummary, BadRequestResponse>> GetInvoicesByCustomerIdWithSummary(
        int customerId,
        QueryStringParameters queryParameters,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching invoices by customer ID from the API /invoices/client/{customerId}", customerId);
        var response = await _httpClient.GetAsync(
                $"/invoices/client/{customerId}?pageNumber={queryParameters.PageNumber}&pageSize={queryParameters.PageSize}&sortOrder={queryParameters.SortOrder}&sortProprety={queryParameters.SortProprety}",
                cancellationToken: cancellationToken);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            var invoices = JsonConvert.DeserializeObject<GetInvoiceListWithSummary>(responseContent);
            return invoices;
        }
        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            return await response.ReadJsonAsync<BadRequestResponse>();
        }
        throw new Exception($"Invoices: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
    }

    public async Task<PagedList<InvoiceResponse>> GetInvoices(QueryStringParameters queryParameters, CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync(
            $"/invoices?pageNumber={queryParameters.PageNumber}&pageSize={queryParameters.PageSize}&searchKeyword={queryParameters.SearchKeyword}",
            cancellationToken: cancellationToken);

        var responseContent = await response.Content.ReadAsStringAsync();
        var pagedInvoices = JsonConvert.DeserializeObject<PagedList<InvoiceResponse>>(responseContent);

        if (response.Headers.TryGetValues("X-Pagination", out var headerValues))
        {
            var paginationMetadata = JsonConvert.DeserializeObject<PaginationMetadata>(headerValues.FirstOrDefault());

            pagedInvoices.TotalCount = paginationMetadata.TotalCount;
            pagedInvoices.PageSize = paginationMetadata.PageSize;
            pagedInvoices.TotalPages = paginationMetadata.TotalPages;
            pagedInvoices.CurrentPage = paginationMetadata.CurrentPage;

            return pagedInvoices;
        }
        return pagedInvoices;
    }

    public async Task<OneOf<int, BadRequestResponse>> CreateInvoice(
        CreateInvoiceRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _httpClient.PostAsJsonAsync("invoices", request, cancellationToken: cancellationToken);
        if (response.StatusCode == HttpStatusCode.Created)
        {
            // Extract invoice number from Location header (format: /invoices/{number})
            if (response.Headers.Location != null)
            {
                var locationPath = response.Headers.Location.ToString();
                var segments = locationPath.Split('/', StringSplitOptions.RemoveEmptyEntries);
                if (segments.Length >= 2 && int.TryParse(segments[segments.Length - 1], out var invoiceNumber))
                {
                    return invoiceNumber;
                }
            }
            throw new Exception($"Invoices: Unable to extract invoice number from Location header: {response.Headers.Location}");
        }
        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            return await response.ReadJsonAsync<BadRequestResponse>();
        }
        throw new Exception($"Invoices: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
    }

    public async Task<OneOf<IList<InvoiceResponse>, BadRequestResponse>> GetInvoicesByIdsAsync(
      List<int> invoiceIds,
      CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching invoices by IDs from the API /invoices/byids");

        var request = new HttpRequestMessage(HttpMethod.Post, "/invoices/byids")
        {
            Content = JsonContent.Create(invoiceIds)
        };

        var response = await _httpClient.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var invoices = JsonConvert.DeserializeObject<IList<InvoiceResponse>>(responseContent) ?? throw new ArgumentNullException("invoices_for_retenu_notfound");
            return invoices.ToList();
        }

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var badRequestResponse = JsonConvert.DeserializeObject<BadRequestResponse>(responseContent);
            return badRequestResponse;
        }

        throw new Exception($"Invoices: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync(cancellationToken)}");
    }

    public async Task<Result<FullInvoiceResponse>> GetFullInvoiceByIdAsync(
    int id,
    CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync(
            $"/invoices/{id}/full",
            cancellationToken: cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return Result.Fail("invoice_not_found");
        }

        _ = response.EnsureSuccessStatusCode();

        var invoice = await response.Content.ReadFromJsonAsync<FullInvoiceResponse>(
            cancellationToken: cancellationToken);

        return invoice is null
            ? Result.Fail<FullInvoiceResponse>("invoice_is_empty")
            : Result.Ok(invoice);
    }

    public async Task<GetInvoicesWithSummariesResponse> GetInvoicesWithSummariesAsync(
        int? customerId,
        string? sortOrder,
        string? sortProperty,
        int pageNumber,
        int pageSize,
        string? searchKeyword,
        DateTime? startDate,
        DateTime? endDate,
        CancellationToken cancellationToken)
    {
        // Validate pagination parameters
        if (pageNumber < 1 || pageSize < 1)
        {
            throw new ArgumentException("PageNumber and PageSize must be greater than zero.");
        }

        // Build query string with only non-null parameters
        var queryParams = new Dictionary<string, string>();

        if (customerId.HasValue)
            queryParams.Add("customerId", customerId.Value.ToString());
        if (!string.IsNullOrEmpty(sortOrder))
            queryParams.Add("sortOrder", sortOrder);
        if (!string.IsNullOrEmpty(sortProperty))
            queryParams.Add("sortProperty", sortProperty);
        if (!string.IsNullOrEmpty(searchKeyword))
            queryParams.Add("searchKeyword", searchKeyword);
        if (startDate.HasValue)
            queryParams.Add("startDate", startDate.Value.ToString("yyyy-MM-dd"));
        if (endDate.HasValue)
            queryParams.Add("endDate", endDate.Value.ToString("yyyy-MM-dd"));

        queryParams.Add("pageNumber", pageNumber.ToString());
        queryParams.Add("pageSize", pageSize.ToString());

        // Construct query string with URL encoding
        var queryString = string.Join("&", queryParams.Select(p => $"{p.Key}={Uri.EscapeDataString(p.Value)}"));
        var requestUri = $"/invoices/summaries?{queryString}";

        try
        {
            // Make the HTTP GET request
            var response = await _httpClient.GetAsync(requestUri, cancellationToken);
            _ = response.EnsureSuccessStatusCode();

            // Deserialize the response
            var summariesResponse = await response.Content.ReadFromJsonAsync<GetInvoicesWithSummariesResponse>(
                cancellationToken: cancellationToken);

            return summariesResponse ?? throw new InvalidOperationException("Failed to deserialize the response.");
        }
        catch (System.Text.Json.JsonException ex)
        {
            throw new InvalidOperationException("Failed to deserialize invoices summaries response.", ex);
        }
    }

    public async Task<Result> ValidateInvoicesAsync(List<int> ids, CancellationToken cancellationToken)
    {
        try
        {
            var request = new ValidateInvoicesRequest { Ids = ids };
            var response = await _httpClient.PostAsJsonAsync("api/invoices/validate", request, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                return Result.Ok();
            }

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var problemDetails = await response.Content.ReadFromJsonAsync<BadRequestResponse>(
                    cancellationToken);

                if (problemDetails?.errors != null)
                {
                    var errors = problemDetails.errors
                        .SelectMany(kvp => kvp.Value.Select(v => $"{kvp.Key}: {v}"));
                    return Result.Fail(errors);
                }
                return Result.Fail("Validation failed but no error details provided");
            }

            return Result.Fail($"Failed to validate invoices: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            return Result.Fail($"Unexpected error: {ex.Message}");
        }
    }

    public async Task<Result<int>> GetInvoiceIdByNumberAsync(
        int invoiceNumber,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching invoice ID by number {InvoiceNumber} from the API /invoices/{InvoiceNumber}/id", invoiceNumber, invoiceNumber);
        
        var response = await _httpClient.GetAsync(
            $"/invoices/{invoiceNumber}/id",
            cancellationToken: cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return Result.Fail("invoice_not_found");
        }

        _ = response.EnsureSuccessStatusCode();

        var invoiceId = await response.Content.ReadFromJsonAsync<int>(
            cancellationToken: cancellationToken);

        return Result.Ok(invoiceId);
    }
}