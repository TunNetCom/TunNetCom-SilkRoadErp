using TunNetCom.SilkRoadErp.Sales.Contracts.FactureAvoirFournisseur;

namespace TunNetCom.SilkRoadErp.Sales.HttpClients.Services.FactureAvoirFournisseur;

public class FactureAvoirFournisseurApiClient : IFactureAvoirFournisseurApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<FactureAvoirFournisseurApiClient> _logger;

    public FactureAvoirFournisseurApiClient(HttpClient httpClient, ILogger<FactureAvoirFournisseurApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<OneOf<int, BadRequestResponse>> CreateFactureAvoirFournisseur(
        CreateFactureAvoirFournisseurRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating facture avoir fournisseur via API /facture-avoir-fournisseur");
        var response = await _httpClient.PostAsJsonAsync("/facture-avoir-fournisseur", request, cancellationToken: cancellationToken);

        if (response.StatusCode == HttpStatusCode.Created)
        {
            if (response.Headers.Location != null)
            {
                var locationPath = response.Headers.Location.ToString();
                var segments = locationPath.Split('/', StringSplitOptions.RemoveEmptyEntries);
                if (segments.Length >= 2 && int.TryParse(segments[segments.Length - 1], out var factureNum))
                {
                    return factureNum;
                }
            }
            throw new Exception($"FactureAvoirFournisseur: Unable to extract facture number from Location header: {response.Headers.Location}");
        }

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            return await response.ReadJsonAsync<BadRequestResponse>();
        }

        throw new Exception($"FactureAvoirFournisseur: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
    }

    public async Task<Result<FactureAvoirFournisseurResponse>> GetFactureAvoirFournisseurAsync(
        int num,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching facture avoir fournisseur from API /facture-avoir-fournisseur/{num}", num);
        var response = await _httpClient.GetAsync($"/facture-avoir-fournisseur/{num}", cancellationToken: cancellationToken);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            var facture = JsonConvert.DeserializeObject<FactureAvoirFournisseurResponse>(responseContent);
            return Result.Ok(facture!);
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return Result.Fail("facture_avoir_fournisseur_not_found");
        }

        throw new Exception($"FactureAvoirFournisseur: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
    }

    public async Task<Result<FullFactureAvoirFournisseurResponse>> GetFullFactureAvoirFournisseurAsync(
        int num,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching full facture avoir fournisseur from API /facture-avoir-fournisseur/{num}/full", num);
        var response = await _httpClient.GetAsync($"/facture-avoir-fournisseur/{num}/full", cancellationToken: cancellationToken);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            var facture = JsonConvert.DeserializeObject<FullFactureAvoirFournisseurResponse>(responseContent);
            return Result.Ok(facture!);
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return Result.Fail("facture_avoir_fournisseur_not_found");
        }

        throw new Exception($"FactureAvoirFournisseur: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
    }

    public async Task<GetFactureAvoirFournisseurWithSummariesResponse> GetFactureAvoirFournisseurWithSummariesAsync(
        int? idFournisseur,
        int? numFactureFournisseur,
        string? sortOrder,
        string? sortProperty,
        int pageNumber,
        int pageSize,
        string? searchKeyword,
        DateTime? startDate,
        DateTime? endDate,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching facture avoir fournisseurs with summaries from API /facture-avoir-fournisseur/summaries");
        var queryString = $"/facture-avoir-fournisseur/summaries?pageNumber={pageNumber}&pageSize={pageSize}";

        if (idFournisseur.HasValue)
        {
            queryString += $"&idFournisseur={idFournisseur.Value}";
        }

        if (numFactureFournisseur.HasValue)
        {
            queryString += $"&numFactureFournisseur={numFactureFournisseur.Value}";
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
        var result = JsonConvert.DeserializeObject<GetFactureAvoirFournisseurWithSummariesResponse>(responseContent);
        return result!;
    }

    public async Task<Result> UpdateFactureAvoirFournisseurAsync(
        int num,
        UpdateFactureAvoirFournisseurRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating facture avoir fournisseur via API /facture-avoir-fournisseur/{num}", num);
        var response = await _httpClient.PutAsJsonAsync($"/facture-avoir-fournisseur/{num}", request, cancellationToken: cancellationToken);

        if (response.StatusCode == HttpStatusCode.NoContent)
        {
            return Result.Ok();
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return Result.Fail("facture_avoir_fournisseur_not_found");
        }

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            var badRequest = await response.ReadJsonAsync<BadRequestResponse>();
            return Result.Fail($"validation_error: {JsonConvert.SerializeObject(badRequest)}");
        }

        throw new Exception($"FactureAvoirFournisseur: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
    }
}

