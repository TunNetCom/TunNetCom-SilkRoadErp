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

        return paged ?? new PagedList<TiersDepenseFonctionnementResponse>(new List<TiersDepenseFonctionnementResponse>(), 0, 1, 10);
    }

    public async Task<OneOf<TiersDepenseFonctionnementResponse, bool>> GetByIdAsync(
        int id,
        CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync($"/tiers-depenses-fonctionnement/{id}", cancellationToken: cancellationToken);
        if (response.StatusCode == HttpStatusCode.OK)
        {
            return await response.ReadJsonAsync<TiersDepenseFonctionnementResponse>();
        }
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return false;
        }
        throw new Exception($"TiersDepenseFonctionnement: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
    }

    public async Task<OneOf<int, BadRequestResponse>> CreateAsync(
        CreateTiersDepenseFonctionnementRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _httpClient.PostAsJsonAsync("/tiers-depenses-fonctionnement", request, cancellationToken: cancellationToken);
        if (response.StatusCode == HttpStatusCode.Created)
        {
            if (response.Headers.Location != null)
            {
                var locationPath = response.Headers.Location.ToString();
                var segments = locationPath.Split('/', StringSplitOptions.RemoveEmptyEntries);
                if (segments.Length >= 1 && int.TryParse(segments[^1], out var id))
                {
                    return id;
                }
            }
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            var created = JsonConvert.DeserializeObject<CreatedIdResponse>(body);
            if (created?.Id > 0)
                return created.Id;
            throw new Exception($"TiersDepenseFonctionnement: Unable to extract id from response: {await response.Content.ReadAsStringAsync()}");
        }
        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            return await response.ReadJsonAsync<BadRequestResponse>();
        }
        throw new Exception($"TiersDepenseFonctionnement: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
    }

    public async Task<OneOf<ResponseTypes, BadRequestResponse>> UpdateAsync(
        int id,
        UpdateTiersDepenseFonctionnementRequest request,
        CancellationToken cancellationToken)
    {
        var headers = new Dictionary<string, string> { { "Accept", "application/problem+json" } };
        var response = await _httpClient.PutAsJsonAsync($"/tiers-depenses-fonctionnement/{id}", request, headers, cancellationToken);
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
        throw new Exception($"TiersDepenseFonctionnement: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
    }

    private class CreatedIdResponse
    {
        [JsonProperty("id")]
        public int Id { get; set; }
    }
}
