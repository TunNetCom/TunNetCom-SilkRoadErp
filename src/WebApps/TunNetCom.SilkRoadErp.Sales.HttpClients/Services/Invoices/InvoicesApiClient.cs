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


    public async Task<List<InvoiceResponse>> GetInvoicesByCustomerId(int customerId, QueryStringParameters queryParameters, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _httpClient.GetAsync(
                $"/invoices/client/{customerId}?pageNumber={queryParameters.PageNumber}&pageSize={queryParameters.PageSize}",
                cancellationToken: cancellationToken);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var invoices = JsonConvert.DeserializeObject<List<InvoiceResponse>>(responseContent);
                return invoices;
            }
            else
            {
                _logger.LogWarning($"Failed to get invoices for customer {customerId}. Status Code: {response.StatusCode}");
                return new List<InvoiceResponse>();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting invoices for customer {customerId}");
            throw;
        }
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

    public async Task<OneOf<CreateInvoiceRequest, BadRequestResponse>> CreateInvoice(CreateInvoiceRequest request, CancellationToken cancellationToken)
    {
        var response = await _httpClient.PostAsJsonAsync("invoices", request, cancellationToken: cancellationToken);
        if (response.StatusCode == HttpStatusCode.Created)
        {
            return await response.ReadJsonAsync<CreateInvoiceRequest>();
        }
        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            return await response.ReadJsonAsync<BadRequestResponse>();
        }
        throw new Exception($"Invoices: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
    }
}
