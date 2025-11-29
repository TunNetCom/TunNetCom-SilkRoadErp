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
                    return avoirNum;
                }
            }
            throw new Exception($"AvoirFournisseur: Unable to extract avoir number from Location header: {response.Headers.Location}");
        }

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            return await response.ReadJsonAsync<BadRequestResponse>();
        }

        throw new Exception($"AvoirFournisseur: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
    }

    public async Task<Result<AvoirFournisseurResponse>> GetAvoirFournisseurAsync(
        int num,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching avoir fournisseur from API /avoir-fournisseur/{num}", num);
        var response = await _httpClient.GetAsync($"/avoir-fournisseur/{num}", cancellationToken: cancellationToken);

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
        int num,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching full avoir fournisseur from API /avoir-fournisseur/{num}/full", num);
        var response = await _httpClient.GetAsync($"/avoir-fournisseur/{num}/full", cancellationToken: cancellationToken);

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
        int? status = null)
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

        var response = await _httpClient.GetAsync(queryString, cancellationToken: cancellationToken);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<GetAvoirFournisseurWithSummariesResponse>(responseContent);
        return result!;
    }

    public async Task<Result> UpdateAvoirFournisseurAsync(
        int num,
        UpdateAvoirFournisseurRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating avoir fournisseur via API /avoir-fournisseur/{num}", num);
        var response = await _httpClient.PutAsJsonAsync($"/avoir-fournisseur/{num}", request, cancellationToken: cancellationToken);

        if (response.StatusCode == HttpStatusCode.NoContent)
        {
            return Result.Ok();
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return Result.Fail("avoir_fournisseur_not_found");
        }

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            var badRequest = await response.ReadJsonAsync<BadRequestResponse>();
            return Result.Fail($"validation_error: {JsonConvert.SerializeObject(badRequest)}");
        }

        throw new Exception($"AvoirFournisseur: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
    }

    public async Task<Result> ValidateAvoirFournisseursAsync(List<int> ids, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Validating avoir fournisseurs via API /avoir-fournisseurs/validate");
        var response = await _httpClient.PostAsJsonAsync("/avoir-fournisseurs/validate", ids, cancellationToken: cancellationToken);

        if (response.StatusCode == HttpStatusCode.NoContent)
        {
            return Result.Ok();
        }

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            var badRequest = await response.ReadJsonAsync<BadRequestResponse>();
            return Result.Fail(badRequest.Detail ?? badRequest.Title ?? "Unknown error");
        }

        throw new Exception($"AvoirFournisseur validation: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
    }
}

