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
        DateTime? startDate = null,
        DateTime? endDate = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Fetching paged FactureDepense: page {PageNumber}, size {PageSize}, " +
            "tiersDepenseFonctionnementId {TiersDepenseId}, accountingYearId {AccountingYearId}, " +
            "search \"{SearchKeyword}\", startDate {StartDate}, endDate {EndDate}",
            pageNumber, pageSize,
            tiersDepenseFonctionnementId, accountingYearId,
            searchKeyword, startDate?.ToString("yyyy-MM-dd"), endDate?.ToString("yyyy-MM-dd"));

        var query = $"/factures-depenses?pageNumber={pageNumber}&pageSize={pageSize}";
        if (tiersDepenseFonctionnementId.HasValue)
            query += $"&tiersDepenseFonctionnementId={tiersDepenseFonctionnementId.Value}";
        if (accountingYearId.HasValue)
            query += $"&accountingYearId={accountingYearId.Value}";
        if (!string.IsNullOrWhiteSpace(searchKeyword))
            query += $"&searchKeyword={Uri.EscapeDataString(searchKeyword)}";
        if (startDate.HasValue)
            query += $"&startDate={Uri.EscapeDataString(startDate.Value.ToString("yyyy-MM-dd"))}";
        if (endDate.HasValue)
            query += $"&endDate={Uri.EscapeDataString(endDate.Value.ToString("yyyy-MM-dd"))}";

        var response = await _httpClient.GetAsync(query, cancellationToken: cancellationToken);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        var result = JsonConvert.DeserializeObject<GetFacturesDepenseWithSummariesResponse>(content) ?? new GetFacturesDepenseWithSummariesResponse();
        _logger.LogInformation("Fetched {ItemCount} FactureDepense summaries (total: {TotalCount})", result.Items.Count, result.TotalCount);
        return result;
    }

    public async Task<OneOf<FactureDepenseResponse, bool>> GetByIdAsync(
        int id,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching FactureDepense with ID: {Id}", id);

        var response = await _httpClient.GetAsync($"/factures-depenses/{id}", cancellationToken: cancellationToken);
        if (response.StatusCode == HttpStatusCode.OK)
        {
            var result = await response.ReadJsonAsync<FactureDepenseResponse>();
            _logger.LogInformation("Fetched FactureDepense with ID: {Id}", id);
            return result;
        }
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            _logger.LogWarning("FactureDepense with ID: {Id} not found", id);
            return false;
        }
        _logger.LogError("Unexpected response fetching FactureDepense {Id}: {StatusCode}", id, response.StatusCode);
        throw new Exception($"FactureDepense: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
    }

    public async Task<OneOf<int, BadRequestResponse>> CreateAsync(
        CreateFactureDepenseRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating FactureDepense with request: {@Request}", request);

        var response = await _httpClient.PostAsJsonAsync("/factures-depenses", request, cancellationToken: cancellationToken);
        if (response.StatusCode == HttpStatusCode.Created)
        {
            if (response.Headers.Location != null)
            {
                var segments = response.Headers.Location.ToString().Split('/', StringSplitOptions.RemoveEmptyEntries);
                if (segments.Length >= 1 && int.TryParse(segments[^1], out var id))
                {
                    _logger.LogInformation("FactureDepense created successfully with ID: {Id}", id);
                    return id;
                }
            }
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            var created = JsonConvert.DeserializeObject<CreatedIdResponse>(body);
            if (created?.Id > 0)
            {
                _logger.LogInformation("FactureDepense created successfully with ID: {Id}", created.Id);
                return created.Id;
            }
            _logger.LogError("Unable to extract id from FactureDepense create response");
            throw new Exception($"FactureDepense: Unable to extract id from response");
        }
        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            _logger.LogWarning("FactureDepense create returned BadRequest: {@Request}", request);
            return await response.ReadJsonAsync<BadRequestResponse>();
        }
        _logger.LogError("Unexpected response creating FactureDepense: {StatusCode}", response.StatusCode);
        throw new Exception($"FactureDepense: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
    }

    public async Task<OneOf<ResponseTypes, BadRequestResponse>> UpdateAsync(
        int id,
        UpdateFactureDepenseRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating FactureDepense with ID: {Id}", id);

        var headers = new Dictionary<string, string> { { "Accept", "application/problem+json" } };
        var response = await _httpClient.PutAsJsonAsync($"/factures-depenses/{id}", request, headers, cancellationToken);
        if (response.StatusCode == HttpStatusCode.NoContent)
        {
            _logger.LogInformation("FactureDepense with ID: {Id} updated successfully", id);
            return ResponseTypes.Success;
        }
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            _logger.LogWarning("FactureDepense with ID: {Id} not found for update", id);
            return ResponseTypes.NotFound;
        }
        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            _logger.LogWarning("FactureDepense update {Id} returned BadRequest", id);
            return await response.ReadJsonAsync<BadRequestResponse>();
        }
        _logger.LogError("Unexpected response updating FactureDepense {Id}: {StatusCode}", id, response.StatusCode);
        throw new Exception($"FactureDepense: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
    }

    public async Task<OneOf<ResponseTypes, BadRequestResponse>> ValidateAsync(
        int id,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Validating FactureDepense with ID: {Id}", id);

        var response = await _httpClient.PostAsync($"/factures-depenses/{id}/validate", cancellationToken: cancellationToken);
        if (response.StatusCode == HttpStatusCode.NoContent || response.StatusCode == HttpStatusCode.OK)
        {
            _logger.LogInformation("FactureDepense with ID: {Id} validated successfully", id);
            return ResponseTypes.Success;
        }
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            _logger.LogWarning("FactureDepense with ID: {Id} not found for validation", id);
            return ResponseTypes.NotFound;
        }
        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            _logger.LogWarning("FactureDepense validate {Id} returned BadRequest", id);
            return await response.ReadJsonAsync<BadRequestResponse>();
        }
        _logger.LogError("Unexpected response validating FactureDepense {Id}: {StatusCode}", id, response.StatusCode);
        throw new Exception($"FactureDepense Validate: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
    }

    public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting FactureDepense with ID: {Id}", id);

        var response = await _httpClient.DeleteAsync($"/factures-depenses/{id}", cancellationToken);
        if (response.StatusCode == HttpStatusCode.NoContent)
        {
            _logger.LogInformation("FactureDepense with ID: {Id} deleted successfully", id);
            return Result.Ok();
        }
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            _logger.LogWarning("FactureDepense with ID: {Id} not found for deletion", id);
            return Result.Fail("facture_depense_not_found");
        }
        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            try
            {
                var problem = JsonConvert.DeserializeObject<ValidationProblemDetails>(content);
                var msg = problem?.Errors?.Values?.FirstOrDefault()?.FirstOrDefault() ?? problem?.Detail ?? "facture_has_paiement";
                _logger.LogWarning("FactureDepense delete {Id} returned BadRequest: {Msg}", id, msg);
                return Result.Fail(msg);
            }
            catch
            {
                _logger.LogWarning("FactureDepense delete {Id} returned BadRequest", id);
                return Result.Fail("facture_has_paiement");
            }
        }
        _logger.LogError("Unexpected response deleting FactureDepense {Id}: {StatusCode}", id, response.StatusCode);
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
