using FluentResults;
using TunNetCom.SilkRoadErp.Sales.Contracts.FactureDepense;
using TunNetCom.SilkRoadErp.Sales.HttpClients;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services;

namespace TunNetCom.SilkRoadErp.Sales.HttpClients.Services.FactureDepense;

public class FactureDepenseApiClient : IFactureDepenseApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<FactureDepenseApiClient> _logger;

    public FactureDepenseApiClient(HttpClient httpClient, ILogger<FactureDepenseApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<GetFacturesDepenseWithSummariesResponse> GetWithSummariesAsync(
        int pageNumber,
        int pageSize,
        int? tiersDepenseFonctionnementId,
        int? accountingYearId,
        string? searchKeyword,
        CancellationToken cancellationToken)
    {
        var query = $"/factures-depenses?pageNumber={pageNumber}&pageSize={pageSize}";
        if (tiersDepenseFonctionnementId.HasValue)
            query += $"&tiersDepenseFonctionnementId={tiersDepenseFonctionnementId.Value}";
        if (accountingYearId.HasValue)
            query += $"&accountingYearId={accountingYearId.Value}";
        if (!string.IsNullOrWhiteSpace(searchKeyword))
            query += $"&searchKeyword={Uri.EscapeDataString(searchKeyword)}";

        var response = await _httpClient.GetAsync(query, cancellationToken: cancellationToken);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonConvert.DeserializeObject<GetFacturesDepenseWithSummariesResponse>(content) ?? new GetFacturesDepenseWithSummariesResponse();
    }

    public async Task<OneOf<FactureDepenseResponse, bool>> GetByIdAsync(
        int id,
        CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync($"/factures-depenses/{id}", cancellationToken: cancellationToken);
        if (response.StatusCode == HttpStatusCode.OK)
        {
            return await response.ReadJsonAsync<FactureDepenseResponse>();
        }
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return false;
        }
        throw new Exception($"FactureDepense: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
    }

    public async Task<OneOf<int, BadRequestResponse>> CreateAsync(
        CreateFactureDepenseRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _httpClient.PostAsJsonAsync("/factures-depenses", request, cancellationToken: cancellationToken);
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
            throw new Exception($"FactureDepense: Unable to extract id from response");
        }
        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            return await response.ReadJsonAsync<BadRequestResponse>();
        }
        throw new Exception($"FactureDepense: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
    }

    public async Task<OneOf<ResponseTypes, BadRequestResponse>> UpdateAsync(
        int id,
        UpdateFactureDepenseRequest request,
        CancellationToken cancellationToken)
    {
        var headers = new Dictionary<string, string> { { "Accept", "application/problem+json" } };
        var response = await _httpClient.PutAsJsonAsync($"/factures-depenses/{id}", request, headers, cancellationToken);
        if (response.StatusCode == HttpStatusCode.NoContent)
            return ResponseTypes.Success;
        if (response.StatusCode == HttpStatusCode.NotFound)
            return ResponseTypes.NotFound;
        if (response.StatusCode == HttpStatusCode.BadRequest)
            return await response.ReadJsonAsync<BadRequestResponse>();
        throw new Exception($"FactureDepense: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
    }

    public async Task<OneOf<ResponseTypes, BadRequestResponse>> ValidateAsync(
        int id,
        CancellationToken cancellationToken)
    {
        var response = await _httpClient.PostAsync($"/factures-depenses/{id}/validate", cancellationToken: cancellationToken);
        if (response.StatusCode == HttpStatusCode.NoContent || response.StatusCode == HttpStatusCode.OK)
            return ResponseTypes.Success;
        if (response.StatusCode == HttpStatusCode.NotFound)
            return ResponseTypes.NotFound;
        if (response.StatusCode == HttpStatusCode.BadRequest)
            return await response.ReadJsonAsync<BadRequestResponse>();
        throw new Exception($"FactureDepense Validate: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
    }

    public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var response = await _httpClient.DeleteAsync($"/factures-depenses/{id}", cancellationToken);
        if (response.StatusCode == HttpStatusCode.NoContent)
            return Result.Ok();
        if (response.StatusCode == HttpStatusCode.NotFound)
            return Result.Fail("facture_depense_not_found");
        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            try
            {
                var problem = JsonConvert.DeserializeObject<ValidationProblemDetails>(content);
                var msg = problem?.Errors?.Values?.FirstOrDefault()?.FirstOrDefault() ?? problem?.Detail ?? "facture_has_paiement";
                return Result.Fail(msg);
            }
            catch
            {
                return Result.Fail("facture_has_paiement");
            }
        }
        throw new Exception($"FactureDepense Delete: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
    }

    private class ValidationProblemDetails
    {
        [JsonProperty("detail")]
        public string? Detail { get; set; }
        [JsonProperty("errors")]
        public Dictionary<string, string[]>? Errors { get; set; }
    }

    private class CreatedIdResponse
    {
        [JsonProperty("id")]
        public int Id { get; set; }
    }
}
