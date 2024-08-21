using TunNetCom.SilkRoadErp.Sales.Contracts.Providers;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services;

namespace TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Providers;

public class ProvidersApiClient : IProvidersApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ProvidersApiClient> _logger;
    public ProvidersApiClient(HttpClient httpClient, ILogger<ProvidersApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<OneOf<ResponseTypes, BadRequestResponse>> UpdateAsync(
        UpdateProviderRequest request,
        int id,
        CancellationToken cancellationToken)
    {
        try
        {
            var headers = new Dictionary<string, string>()
            {
                { $"Accept", $"application/problem+json" }
            };

            var response = await _httpClient.PutAsJsonAsync($"{id}", request, headers, cancellationToken);
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
            throw new Exception($"Providersid: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            throw;
        }
    }

    public async Task<OneOf<ProviderResponse, bool>> GetAsync(
        int id,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{id}", cancellationToken: cancellationToken);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return await response.ReadJsonAsync<ProviderResponse>();
            }
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return false;
            }
            throw new Exception($"Providersid: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
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
            var response = await _httpClient.DeleteAsync($"{id.ToString()}", cancellationToken: cancellationToken);
            if (response.StatusCode is HttpStatusCode.NoContent or HttpStatusCode.NotFound)
            {
                return await response.Content.ReadAsStreamAsync(cancellationToken);
            }
            throw new Exception($"Providersid: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            throw;
        }
    }

    public async Task<PagedList<ProviderResponse>> GetPagedAsync(
        QueryStringParameters queryParameters,
        CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync(
            $"/providers?pageNumber={queryParameters.PageNumber}&pageSize={queryParameters.PageSize}&searchKeyword={queryParameters.SearchKeyword}",
            cancellationToken: cancellationToken);

        var responseContent = await response.Content.ReadAsStringAsync();
        var pagedProviders = JsonConvert.DeserializeObject<PagedList<ProviderResponse>>(responseContent);

        if (response.Headers.TryGetValues("X-Pagination", out var headerValues))
        {
            var paginationMetadata = JsonConvert.DeserializeObject<PaginationMetadata>(headerValues.FirstOrDefault());

            pagedProviders.TotalCount = paginationMetadata.TotalCount;
            pagedProviders.PageSize = paginationMetadata.PageSize;
            pagedProviders.TotalPages = paginationMetadata.TotalPages;
            pagedProviders.CurrentPage = paginationMetadata.CurrentPage;

            return pagedProviders;
        }
        return pagedProviders;
    }

    public async Task<OneOf<CreateProviderRequest, BadRequestResponse>> CreateAsync(
        CreateProviderRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _httpClient.PostAsJsonAsync($"", request, cancellationToken: cancellationToken);
        if (response.StatusCode == HttpStatusCode.Created)
        {
            return await response.ReadJsonAsync<CreateProviderRequest>();
        }
        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            return await response.ReadJsonAsync<BadRequestResponse>();
        }
        throw new Exception($"Providers: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
    }
}