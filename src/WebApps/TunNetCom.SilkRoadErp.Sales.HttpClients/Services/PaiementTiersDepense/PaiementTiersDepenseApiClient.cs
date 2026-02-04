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
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var query = $"/paiements-tiers-depenses?pageNumber={pageNumber}&pageSize={pageSize}";
        if (tiersDepenseFonctionnementId.HasValue)
            query += $"&tiersDepenseFonctionnementId={tiersDepenseFonctionnementId.Value}";
        if (accountingYearId.HasValue)
            query += $"&accountingYearId={accountingYearId.Value}";

        var response = await _httpClient.GetAsync(query, cancellationToken: cancellationToken);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonConvert.DeserializeObject<PagedList<PaiementTiersDepenseResponse>>(content) ?? new PagedList<PaiementTiersDepenseResponse>(new List<PaiementTiersDepenseResponse>(), 0, pageNumber, pageSize);
    }

    public async Task<OneOf<PaiementTiersDepenseResponse, bool>> GetByIdAsync(
        int id,
        CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync($"/paiements-tiers-depenses/{id}", cancellationToken: cancellationToken);
        if (response.StatusCode == HttpStatusCode.OK)
        {
            return await response.ReadJsonAsync<PaiementTiersDepenseResponse>();
        }
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return false;
        }
        throw new Exception($"PaiementTiersDepense: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
    }

    public async Task<OneOf<int, BadRequestResponse>> CreateAsync(
        CreatePaiementTiersDepenseRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _httpClient.PostAsJsonAsync("/paiements-tiers-depenses", request, cancellationToken: cancellationToken);
        if (response.StatusCode == HttpStatusCode.Created)
        {
            if (response.Headers.Location != null)
            {
                var segments = response.Headers.Location.ToString().Split('/', StringSplitOptions.RemoveEmptyEntries);
                if (segments.Length >= 1 && int.TryParse(segments[^1], out var id))
                    return id;
            }
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            var created = JsonConvert.DeserializeObject<CreatedIdResponse>(body);
            if (created?.Id > 0)
                return created.Id;
            throw new Exception($"PaiementTiersDepense: Unable to extract id from response");
        }
        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            return await response.ReadJsonAsync<BadRequestResponse>();
        }
        throw new Exception($"PaiementTiersDepense: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
    }

    public async Task<OneOf<ResponseTypes, BadRequestResponse>> UpdateAsync(
        int id,
        UpdatePaiementTiersDepenseRequest request,
        CancellationToken cancellationToken)
    {
        var headers = new Dictionary<string, string> { { "Accept", "application/problem+json" } };
        var response = await _httpClient.PutAsJsonAsync($"/paiements-tiers-depenses/{id}", request, headers, cancellationToken);
        if (response.StatusCode == HttpStatusCode.NoContent)
            return ResponseTypes.Success;
        if (response.StatusCode == HttpStatusCode.NotFound)
            return ResponseTypes.NotFound;
        if (response.StatusCode == HttpStatusCode.BadRequest)
            return await response.ReadJsonAsync<BadRequestResponse>();
        throw new Exception($"PaiementTiersDepense: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
    }

    public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var response = await _httpClient.DeleteAsync($"/paiements-tiers-depenses/{id}", cancellationToken);
        if (response.StatusCode == HttpStatusCode.NoContent)
            return Result.Ok();
        if (response.StatusCode == HttpStatusCode.NotFound)
            return Result.Fail("paiement_tiers_depense_not_found");
        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            return Result.Fail(content);
        }
        throw new Exception($"PaiementTiersDepense Delete: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
    }

    private class CreatedIdResponse
    {
        [JsonProperty("id")]
        public int Id { get; set; }
    }
}
