using FluentResults;
using TunNetCom.SilkRoadErp.Sales.Contracts;
using TunNetCom.SilkRoadErp.Sales.Contracts.PaiementTiersDepense;
using TunNetCom.SilkRoadErp.Sales.HttpClients;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services;

namespace TunNetCom.SilkRoadErp.Sales.HttpClients.Services.PaiementTiersDepense;

public class PaiementTiersDepenseApiClient : IPaiementTiersDepenseApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<PaiementTiersDepenseApiClient> _logger;

    public PaiementTiersDepenseApiClient(HttpClient httpClient, ILogger<PaiementTiersDepenseApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<PagedList<PaiementTiersDepenseResponse>> GetPagedAsync(
        int? tiersDepenseFonctionnementId,
        int? accountingYearId,
        DateTime? datePaiementFrom = null,
        DateTime? datePaiementTo = null,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Fetching paged PaiementTiersDepense: page {PageNumber}, size {PageSize}, " +
            "tiersDepenseFonctionnementId {TiersDepenseId}, accountingYearId {AccountingYearId}, " +
            "dateFrom {DateFrom}, dateTo {DateTo}",
            pageNumber, pageSize,
            tiersDepenseFonctionnementId, accountingYearId,
            datePaiementFrom?.ToString("yyyy-MM-dd"), datePaiementTo?.ToString("yyyy-MM-dd"));

        var query = $"/paiements-tiers-depenses?pageNumber={pageNumber}&pageSize={pageSize}";
        if (tiersDepenseFonctionnementId.HasValue)
            query += $"&tiersDepenseFonctionnementId={tiersDepenseFonctionnementId.Value}";
        if (accountingYearId.HasValue)
            query += $"&accountingYearId={accountingYearId.Value}";
        if (datePaiementFrom.HasValue)
            query += $"&datePaiementFrom={Uri.EscapeDataString(datePaiementFrom.Value.ToString("yyyy-MM-dd"))}";
        if (datePaiementTo.HasValue)
            query += $"&datePaiementTo={Uri.EscapeDataString(datePaiementTo.Value.ToString("yyyy-MM-dd"))}";

        var response = await _httpClient.GetAsync(query, cancellationToken: cancellationToken);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        var paged = JsonConvert.DeserializeObject<PagedList<PaiementTiersDepenseResponse>>(content);
        var result = paged ?? new PagedList<PaiementTiersDepenseResponse>(new List<PaiementTiersDepenseResponse>(), 0, pageNumber, pageSize);
        _logger.LogInformation("Fetched {ItemCount} PaiementTiersDepense (total: {TotalCount})", result.Items.Count, result.TotalCount);
        return result;
    }

    public async Task<OneOf<PaiementTiersDepenseResponse, bool>> GetByIdAsync(
        int id,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching PaiementTiersDepense with ID: {Id}", id);

        var response = await _httpClient.GetAsync($"/paiements-tiers-depenses/{id}", cancellationToken: cancellationToken);
        if (response.StatusCode == HttpStatusCode.OK)
        {
            var result = await response.ReadJsonAsync<PaiementTiersDepenseResponse>();
            _logger.LogInformation("Fetched PaiementTiersDepense with ID: {Id}", id);
            return result;
        }
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            _logger.LogWarning("PaiementTiersDepense with ID: {Id} not found", id);
            return false;
        }
        _logger.LogError("Unexpected response fetching PaiementTiersDepense {Id}: {StatusCode}", id, response.StatusCode);
        throw new Exception($"PaiementTiersDepense: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
    }

    public async Task<OneOf<int, BadRequestResponse>> CreateAsync(
        CreatePaiementTiersDepenseRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating PaiementTiersDepense with request: {@Request}", request);

        var response = await _httpClient.PostAsJsonAsync("/paiements-tiers-depenses", request, cancellationToken: cancellationToken);
        if (response.StatusCode == HttpStatusCode.Created)
        {
            if (response.Headers.Location != null)
            {
                var segments = response.Headers.Location.ToString().Split('/', StringSplitOptions.RemoveEmptyEntries);
                if (segments.Length >= 1 && int.TryParse(segments[^1], out var id))
                {
                    _logger.LogInformation("PaiementTiersDepense created successfully with ID: {Id}", id);
                    return id;
                }
            }
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            var created = JsonConvert.DeserializeObject<CreatedIdResponse>(body);
            if (created?.Id > 0)
            {
                _logger.LogInformation("PaiementTiersDepense created successfully with ID: {Id}", created.Id);
                return created.Id;
            }
            _logger.LogError("Unable to extract id from PaiementTiersDepense create response");
            throw new Exception($"PaiementTiersDepense: Unable to extract id from response");
        }
        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            _logger.LogWarning("PaiementTiersDepense create returned BadRequest: {@Request}", request);
            return await response.ReadJsonAsync<BadRequestResponse>();
        }
        _logger.LogError("Unexpected response creating PaiementTiersDepense: {StatusCode}", response.StatusCode);
        throw new Exception($"PaiementTiersDepense: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
    }

    public async Task<OneOf<ResponseTypes, BadRequestResponse>> UpdateAsync(
        int id,
        UpdatePaiementTiersDepenseRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating PaiementTiersDepense with ID: {Id}", id);

        var headers = new Dictionary<string, string> { { "Accept", "application/problem+json" } };
        var response = await _httpClient.PutAsJsonAsync($"/paiements-tiers-depenses/{id}", request, headers, cancellationToken);
        if (response.StatusCode == HttpStatusCode.NoContent)
        {
            _logger.LogInformation("PaiementTiersDepense with ID: {Id} updated successfully", id);
            return ResponseTypes.Success;
        }
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            _logger.LogWarning("PaiementTiersDepense with ID: {Id} not found for update", id);
            return ResponseTypes.NotFound;
        }
        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            _logger.LogWarning("PaiementTiersDepense update {Id} returned BadRequest", id);
            return await response.ReadJsonAsync<BadRequestResponse>();
        }
        _logger.LogError("Unexpected response updating PaiementTiersDepense {Id}: {StatusCode}", id, response.StatusCode);
        throw new Exception($"PaiementTiersDepense: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
    }

    public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting PaiementTiersDepense with ID: {Id}", id);

        var response = await _httpClient.DeleteAsync($"/paiements-tiers-depenses/{id}", cancellationToken);
        if (response.StatusCode == HttpStatusCode.NoContent)
        {
            _logger.LogInformation("PaiementTiersDepense with ID: {Id} deleted successfully", id);
            return Result.Ok();
        }
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            _logger.LogWarning("PaiementTiersDepense with ID: {Id} not found for deletion", id);
            return Result.Fail("paiement_tiers_depense_not_found");
        }
        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogWarning("PaiementTiersDepense delete {Id} returned BadRequest: {Content}", id, content);
            return Result.Fail(content);
        }
        _logger.LogError("Unexpected response deleting PaiementTiersDepense {Id}: {StatusCode}", id, response.StatusCode);
        throw new Exception($"PaiementTiersDepense Delete: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
    }

    private class CreatedIdResponse
    {
        [JsonProperty("id")]
        public int Id { get; set; }
    }
}
