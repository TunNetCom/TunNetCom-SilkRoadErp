using TunNetCom.SilkRoadErp.Sales.Contracts.Customers;

namespace TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Customers;

public class CustomersApiClient : ICustomersApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<CustomersApiClient> _logger;
    public CustomersApiClient(HttpClient httpClient, ILogger<CustomersApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<OneOf<ResponseTypes, BadRequestResponse>> UpdateAsync(
        UpdateCustomerRequest request,
        int id,
        CancellationToken cancellationToken)
    {
        try
        {
            var headers = new Dictionary<string, string>()
            {
                { $"Accept", $"application/problem+json" }
            };

            var response = await _httpClient.PutAsJsonAsync($"/customers/{id}", request, headers, cancellationToken);
            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                return ResponseTypes.Success;
            }
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return ResponseTypes.NotFound;
            }
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                return await response.ReadJsonAsync<BadRequestResponse>();
            }
            throw new Exception($"Customersid: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            throw;
        }
    }

    public async Task<OneOf<CustomerResponse, bool>> GetAsync(
        int id,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/customers/{id}", cancellationToken: cancellationToken);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return await response.ReadJsonAsync<CustomerResponse>();
            }
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return false;
            }
            throw new Exception($"Customersid: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            throw;
        }
    }

    public async Task<Stream> DeleteAsync(
        int id,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"/customers/{id.ToString()}", cancellationToken: cancellationToken);
            if (response.StatusCode is HttpStatusCode.NoContent or HttpStatusCode.NotFound)
            {
                return await response.Content.ReadAsStreamAsync(cancellationToken);
            }
            throw new Exception($"Customersid: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            throw;
        }
    }

    public async Task<PagedList<CustomerResponse>> GetAsync(
        QueryStringParameters queryParameters,
        CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync(
            $"/customers?pageNumber={queryParameters.PageNumber}&pageSize={queryParameters.PageSize}&searchKeyword={queryParameters.SearchKeyword}",
            cancellationToken: cancellationToken);

        var responseContent = await response.Content.ReadAsStringAsync();
        var pagedClients = JsonConvert.DeserializeObject<PagedList<CustomerResponse>>(responseContent);

        if (response.Headers.TryGetValues("X-Pagination", out var headerValues))
        {
            var paginationMetadata = JsonConvert.DeserializeObject<PaginationMetadata>(headerValues.FirstOrDefault());

            pagedClients.TotalCount = paginationMetadata.TotalCount;
            pagedClients.PageSize = paginationMetadata.PageSize;
            pagedClients.TotalPages = paginationMetadata.TotalPages;
            pagedClients.CurrentPage = paginationMetadata.CurrentPage;

            return pagedClients;
        }
        return pagedClients;
    }

    public async Task<OneOf<CreateCustomerRequest, BadRequestResponse>> CreateAsync(
        CreateCustomerRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _httpClient.PostAsJsonAsync($"/customers", request, cancellationToken: cancellationToken);
        if (response.StatusCode == HttpStatusCode.Created)
        {
            return await response.ReadJsonAsync<CreateCustomerRequest>();
        }
        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            return await response.ReadJsonAsync<BadRequestResponse>();
        }
        throw new Exception($"Customers: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
    }

    public async Task<PagedList<CustomerResponse>> SearchCustomers(QueryStringParameters queryParameters, CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync(
             $"/customers?pageNumber={queryParameters.PageNumber}&pageSize={queryParameters.PageSize}&searchKeyword={queryParameters.SearchKeyword}",
            cancellationToken);

        var responseContent = await response.Content.ReadAsStringAsync();
        var pagedCustomers = JsonConvert.DeserializeObject<PagedList<CustomerResponse>>(responseContent);
        return pagedCustomers;
    }

    public async Task<CustomerResponse?> GetCustomerById(int id, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/customers/{id}", cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<CustomerResponse>(cancellationToken: cancellationToken);
            }
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
            throw new Exception($"Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            throw;
        }
    }

}