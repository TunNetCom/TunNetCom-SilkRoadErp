using TunNetCom.SilkRoadErp.Sales.Contracts.AvoirFournisseur;

namespace TunNetCom.SilkRoadErp.Sales.HttpClients.Services.AvoirFournisseur;

public class AvoirFournisseurApiClient : IAvoirFournisseurApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AvoirFournisseurApiClient> _logger;

    public AvoirFournisseurApiClient(HttpClient httpClient, ILogger<AvoirFournisseurApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<OneOf<int, BadRequestResponse>> CreateAvoirFournisseur(
        CreateAvoirFournisseurRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating avoir fournisseur via API /avoir-fournisseur");
        var response = await _httpClient.PostAsJsonAsync("/avoir-fournisseur", request, cancellationToken: cancellationToken);

        if (response.StatusCode == HttpStatusCode.Created)
        {
            if (response.Headers.Location != null)
            {
                var locationPath = response.Headers.Location.ToString();
                var segments = locationPath.Split('/', StringSplitOptions.RemoveEmptyEntries);
                if (segments.Length >= 2 && int.TryParse(segments[segments.Length - 1], out var avoirNum))
                {
                    _logger.LogInformation("AvoirFournisseur created successfully. Status={StatusCode}, Id={Id}", response.StatusCode, avoirNum);
                    return avoirNum;
                }
            }
            var location = response.Headers.Location?.ToString() ?? "null";
            _logger.LogError("AvoirFournisseur create: Unable to extract id from Location header: {Location}", location);
            throw new Exception($"AvoirFournisseur: Unable to extract avoir number from Location header: {response.Headers.Location}");
        }

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogWarning("AvoirFournisseur create BadRequest. Status={StatusCode}, Content={Content}", response.StatusCode, body);
            return await response.ReadJsonAsync<BadRequestResponse>();
        }

        var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
        _logger.LogError("AvoirFournisseur create unexpected response. Status={StatusCode}, Content={Content}", response.StatusCode, errorContent);
        throw new Exception($"AvoirFournisseur: Unexpected response. Status Code: {response.StatusCode}. Content: {errorContent}");
    }

    public async Task<Result<AvoirFournisseurResponse>> GetAvoirFournisseurAsync(
        int id,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching avoir fournisseur from API /avoir-fournisseur/{id}", id);
        var response = await _httpClient.GetAsync($"/avoir-fournisseur/{id}", cancellationToken: cancellationToken);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            var avoir = JsonConvert.DeserializeObject<AvoirFournisseurResponse>(responseContent);
            return Result.Ok(avoir!);
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return Result.Fail("avoir_fournisseur_not_found");
        }

        throw new Exception($"AvoirFournisseur: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
    }

    public async Task<Result<FullAvoirFournisseurResponse>> GetFullAvoirFournisseurAsync(
        int id,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching full avoir fournisseur from API /avoir-fournisseur/{id}/full", id);
        var response = await _httpClient.GetAsync($"/avoir-fournisseur/{id}/full", cancellationToken: cancellationToken);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            var avoir = JsonConvert.DeserializeObject<FullAvoirFournisseurResponse>(responseContent);
            return Result.Ok(avoir!);
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return Result.Fail("avoir_fournisseur_not_found");
        }

        throw new Exception($"AvoirFournisseur: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
    }

    public async Task<GetAvoirFournisseurWithSummariesResponse> GetAvoirFournisseurWithSummariesAsync(
        int? fournisseurId,
        int? numFactureAvoirFournisseur,
        string? sortOrder,
        string? sortProperty,
        int pageNumber,
        int pageSize,
        string? searchKeyword,
        DateTime? startDate,
        DateTime? endDate,
        CancellationToken cancellationToken,
        int? status = null,
        bool onlyUninvoiced = false)
    {
        _logger.LogInformation("Fetching avoir fournisseurs with summaries from API /avoir-fournisseur/summaries");
        var queryString = $"/avoir-fournisseur/summaries?pageNumber={pageNumber}&pageSize={pageSize}";

        if (status.HasValue)
        {
            queryString += $"&status={status.Value}";
        }

        if (fournisseurId.HasValue)
        {
            queryString += $"&fournisseurId={fournisseurId.Value}";
        }

        if (numFactureAvoirFournisseur.HasValue)
        {
            queryString += $"&numFactureAvoirFournisseur={numFactureAvoirFournisseur.Value}";
        }

        if (onlyUninvoiced)
        {
            queryString += "&onlyUninvoiced=true";
        }

        if (!string.IsNullOrEmpty(sortOrder))
        {
            queryString += $"&sortOrder={Uri.EscapeDataString(sortOrder)}";
        }

        if (!string.IsNullOrEmpty(sortProperty))
        {
            queryString += $"&sortProperty={Uri.EscapeDataString(sortProperty)}";
        }

        if (!string.IsNullOrEmpty(searchKeyword))
        {
            queryString += $"&searchKeyword={Uri.EscapeDataString(searchKeyword)}";
        }

        if (startDate.HasValue)
        {
            queryString += $"&startDate={startDate.Value:yyyy-MM-dd}";
        }

        if (endDate.HasValue)
        {
            queryString += $"&endDate={endDate.Value:yyyy-MM-dd}";
        }

        try
        {
            var response = await _httpClient.GetAsync(queryString, cancellationToken: cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogWarning("AvoirFournisseur summaries error. Url={Url}, Status={StatusCode}, Content={Content}", queryString, response.StatusCode, body);
                response.EnsureSuccessStatusCode();
            }

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = JsonConvert.DeserializeObject<GetAvoirFournisseurWithSummariesResponse>(responseContent);
            return result!;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AvoirFournisseur GetAvoirFournisseurWithSummariesAsync failed. Url={Url}", queryString);
            throw;
        }
    }

    public async Task<Result> UpdateAvoirFournisseurAsync(
        int id,
        UpdateAvoirFournisseurRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating avoir fournisseur via API /avoir-fournisseur/{id}", id);
        var response = await _httpClient.PutAsJsonAsync($"/avoir-fournisseur/{id}", request, cancellationToken: cancellationToken);

        if (response.StatusCode == HttpStatusCode.NoContent)
        {
            _logger.LogInformation("AvoirFournisseur updated successfully. Id={Id}, Status={StatusCode}", id, response.StatusCode);
            return Result.Ok();
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            _logger.LogWarning("AvoirFournisseur update not found. Id={Id}", id);
            return Result.Fail("avoir_fournisseur_not_found");
        }

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogWarning("AvoirFournisseur update BadRequest. Id={Id}, Status={StatusCode}, Content={Content}", id, response.StatusCode, body);
            var badRequest = await response.ReadJsonAsync<BadRequestResponse>();
            return Result.Fail($"validation_error: {JsonConvert.SerializeObject(badRequest)}");
        }

        var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
        _logger.LogError("AvoirFournisseur update unexpected response. Id={Id}, Status={StatusCode}, Content={Content}", id, response.StatusCode, errorContent);
        throw new Exception($"AvoirFournisseur: Unexpected response. Status Code: {response.StatusCode}. Content: {errorContent}");
    }

    public async Task<Result> ValidateAvoirFournisseursAsync(List<int> ids, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Validating avoir fournisseurs via API api/avoir-fournisseurs/validate");
        var response = await _httpClient.PostAsJsonAsync("api/avoir-fournisseurs/validate", ids, cancellationToken: cancellationToken);

        if (response.StatusCode == HttpStatusCode.NoContent)
        {
            return Result.Ok();
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return Result.Fail("avoir_fournisseurs_not_found");
        }

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            var badRequest = await response.ReadJsonAsync<BadRequestResponse>();
            return Result.Fail(badRequest.Detail ?? badRequest.Title ?? "Unknown error");
        }

        throw new Exception($"AvoirFournisseur validation: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
    }
}

