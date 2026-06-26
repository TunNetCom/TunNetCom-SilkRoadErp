using TunNetCom.SilkRoadErp.Sales.Contracts;
using TunNetCom.SilkRoadErp.Sales.Contracts.TiersDepenseFonctionnement;
using TunNetCom.SilkRoadErp.Sales.HttpClients;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services;

namespace TunNetCom.SilkRoadErp.Sales.HttpClients.Services.TiersDepenseFonctionnement;

public class TiersDepenseFonctionnementApiClient : ITiersDepenseFonctionnementApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<TiersDepenseFonctionnementApiClient> _logger;

    public TiersDepenseFonctionnementApiClient(HttpClient httpClient, ILogger<TiersDepenseFonctionnementApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<PagedList<TiersDepenseFonctionnementResponse>> GetPagedAsync(
        QueryStringParameters queryParameters,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Fetching paged TiersDepenseFonctionnement: page {PageNumber}, size {PageSize}, search \"{SearchKeyword}\"",
            queryParameters.PageNumber, queryParameters.PageSize, queryParameters.SearchKeyword ?? "");

        var response = await _httpClient.GetAsync(
            $"/tiers-depenses-fonctionnement?pageNumber={queryParameters.PageNumber}&pageSize={queryParameters.PageSize}&searchKeyword={Uri.EscapeDataString(queryParameters.SearchKeyword ?? "")}",
            cancellationToken: cancellationToken);

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        var paged = JsonConvert.DeserializeObject<PagedList<TiersDepenseFonctionnementResponse>>(responseContent);

        if (response.Headers.TryGetValues("X-Pagination", out var headerValues))
        {
            var paginationMetadata = JsonConvert.DeserializeObject<PaginationMetadata>(headerValues.FirstOrDefault() ?? "{}");
            if (paginationMetadata != null && paged != null)
            {
                paged.TotalCount = paginationMetadata.TotalCount;
                paged.PageSize = paginationMetadata.PageSize;
                paged.TotalPages = paginationMetadata.TotalPages;
                paged.CurrentPage = paginationMetadata.CurrentPage;
            }
        }

        var itemCount = paged?.Items.Count ?? 0;
        _logger.LogInformation("Fetched {ItemCount} TiersDepenseFonctionnement (total: {TotalCount})", itemCount, paged?.TotalCount ?? 0);
        return paged ?? new PagedList<TiersDepenseFonctionnementResponse>(new List<TiersDepenseFonctionnementResponse>(), 0, 1, 10);
    }

    public async Task<OneOf<TiersDepenseFonctionnementResponse, bool>> GetByIdAsync(
        int id,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching TiersDepenseFonctionnement with ID: {Id}", id);

        var response = await _httpClient.GetAsync($"/tiers-depenses-fonctionnement/{id}", cancellationToken: cancellationToken);
        if (response.StatusCode == HttpStatusCode.OK)
        {
            var result = await response.ReadJsonAsync<TiersDepenseFonctionnementResponse>();
            _logger.LogInformation("Fetched TiersDepenseFonctionnement with ID: {Id}", id);
            return result;
        }
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            _logger.LogWarning("TiersDepenseFonctionnement with ID: {Id} not found", id);
            return false;
        }
        _logger.LogError("Unexpected response fetching TiersDepenseFonctionnement {Id}: {StatusCode}", id, response.StatusCode);
        throw new Exception($"TiersDepenseFonctionnement: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
    }

    public async Task<OneOf<int, BadRequestResponse>> CreateAsync(
        CreateTiersDepenseFonctionnementRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating TiersDepenseFonctionnement with request: {@Request}", request);

        var response = await _httpClient.PostAsJsonAsync("/tiers-depenses-fonctionnement", request, cancellationToken: cancellationToken);
        if (response.StatusCode == HttpStatusCode.Created)
        {
            if (response.Headers.Location != null)
            {
                var locationPath = response.Headers.Location.ToString();
                var segments = locationPath.Split('/', StringSplitOptions.RemoveEmptyEntries);
                if (segments.Length >= 1 && int.TryParse(segments[^1], out var id))
                {
                    _logger.LogInformation("TiersDepenseFonctionnement created successfully with ID: {Id}", id);
                    return id;
                }
            }
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            var created = JsonConvert.DeserializeObject<CreatedIdResponse>(body);
            if (created?.Id > 0)
            {
                _logger.LogInformation("TiersDepenseFonctionnement created successfully with ID: {Id}", created.Id);
                return created.Id;
            }
            _logger.LogError("Unable to extract id from TiersDepenseFonctionnement create response: {Body}", body);
            throw new Exception($"TiersDepenseFonctionnement: Unable to extract id from response: {await response.Content.ReadAsStringAsync()}");
        }
        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            _logger.LogWarning("TiersDepenseFonctionnement create returned BadRequest: {@Request}", request);
            return await response.ReadJsonAsync<BadRequestResponse>();
        }
        _logger.LogError("Unexpected response creating TiersDepenseFonctionnement: {StatusCode}", response.StatusCode);
        throw new Exception($"TiersDepenseFonctionnement: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
    }

    public async Task<OneOf<ResponseTypes, BadRequestResponse>> UpdateAsync(
        int id,
        UpdateTiersDepenseFonctionnementRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating TiersDepenseFonctionnement with ID: {Id}", id);

        var headers = new Dictionary<string, string> { { "Accept", "application/problem+json" } };
        var response = await _httpClient.PutAsJsonAsync($"/tiers-depenses-fonctionnement/{id}", request, headers, cancellationToken);
        if (response.StatusCode == HttpStatusCode.NoContent)
        {
            _logger.LogInformation("TiersDepenseFonctionnement with ID: {Id} updated successfully", id);
            return ResponseTypes.Success;
        }
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            _logger.LogWarning("TiersDepenseFonctionnement with ID: {Id} not found for update", id);
            return ResponseTypes.NotFound;
        }
        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            _logger.LogWarning("TiersDepenseFonctionnement update {Id} returned BadRequest", id);
            return await response.ReadJsonAsync<BadRequestResponse>();
        }
        _logger.LogError("Unexpected response updating TiersDepenseFonctionnement {Id}: {StatusCode}", id, response.StatusCode);
        throw new Exception($"TiersDepenseFonctionnement: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
    }

    private class CreatedIdResponse
    {
        [JsonProperty("id")]
        public int Id { get; set; }
    }
}
