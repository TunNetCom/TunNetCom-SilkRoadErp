using FluentResults;

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

    public async Task<OneOf<CreateInvoiceRequest, BadRequestResponse>> CreateInvoice(
        CreateInvoiceRequest request,
        CancellationToken cancellationToken)
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

    public async Task<OneOf<IList<InvoiceResponse>, BadRequestResponse>> GetInvoicesByIdsAsync(
      List<int> invoiceIds,
      CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching invoices by IDs from the API /invoices/byids");

        var request = new HttpRequestMessage(HttpMethod.Get, "/invoices/byids")
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

        response.EnsureSuccessStatusCode();

        var invoice = await response.Content.ReadFromJsonAsync<FullInvoiceResponse>(
            cancellationToken: cancellationToken);

        return invoice is null
            ? Result.Fail<FullInvoiceResponse>("invoice_is_empty")
            : Result.Ok(invoice);
    }
}