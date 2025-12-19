using TunNetCom.SilkRoadErp.Sales.Contracts.PaiementFournisseur;
using TunNetCom.SilkRoadErp.Sales.HttpClients;

namespace TunNetCom.SilkRoadErp.Sales.HttpClients.Services.PaiementFournisseur;

public class PaiementFournisseurApiClient : IPaiementFournisseurApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<PaiementFournisseurApiClient> _logger;

    public PaiementFournisseurApiClient(HttpClient httpClient, ILogger<PaiementFournisseurApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<OneOf<int, BadRequestResponse>> CreatePaiementFournisseurAsync(
        CreatePaiementFournisseurRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating paiement fournisseur via API /paiement-fournisseur");
        var response = await _httpClient.PostAsJsonAsync("/paiement-fournisseur", request, cancellationToken: cancellationToken);

        if (response.StatusCode == HttpStatusCode.Created)
        {
            if (response.Headers.Location != null)
            {
                var locationPath = response.Headers.Location.ToString();
                var segments = locationPath.Split('/', StringSplitOptions.RemoveEmptyEntries);
                if (segments.Length >= 2 && int.TryParse(segments[segments.Length - 1], out var paiementId))
                {
                    return paiementId;
                }
            }
            throw new Exception($"PaiementFournisseur: Unable to extract paiement id from Location header: {response.Headers.Location}");
        }

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            return await response.ReadJsonAsync<BadRequestResponse>();
        }

        throw new Exception($"PaiementFournisseur: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
    }

    public async Task<Result<PaiementFournisseurResponse>> GetPaiementFournisseurAsync(
        int id,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching paiement fournisseur from API /paiement-fournisseur/{id}", id);
        var response = await _httpClient.GetAsync($"/paiement-fournisseur/{id}", cancellationToken: cancellationToken);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            var paiement = JsonConvert.DeserializeObject<PaiementFournisseurResponse>(responseContent);
            return Result.Ok(paiement!);
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return Result.Fail("paiement_fournisseur_not_found");
        }

        throw new Exception($"PaiementFournisseur: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
    }

    public async Task<PagedList<PaiementFournisseurResponse>> GetPaiementsFournisseurAsync(
        int? fournisseurId,
        int? accountingYearId,
        DateTime? dateEcheanceFrom,
        DateTime? dateEcheanceTo,
        decimal? montantMin,
        decimal? montantMax,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching paiements fournisseur from API /paiement-fournisseur");
        var queryString = $"/paiement-fournisseur?pageNumber={pageNumber}&pageSize={pageSize}";

        if (fournisseurId.HasValue)
        {
            queryString += $"&fournisseurId={fournisseurId.Value}";
        }

        if (accountingYearId.HasValue)
        {
            queryString += $"&accountingYearId={accountingYearId.Value}";
        }

        if (dateEcheanceFrom.HasValue)
        {
            queryString += $"&dateEcheanceFrom={Uri.EscapeDataString(dateEcheanceFrom.Value.ToString("yyyy-MM-ddTHH:mm:ss"))}";
        }

        if (dateEcheanceTo.HasValue)
        {
            queryString += $"&dateEcheanceTo={Uri.EscapeDataString(dateEcheanceTo.Value.ToString("yyyy-MM-ddTHH:mm:ss"))}";
        }

        if (montantMin.HasValue)
        {
            queryString += $"&montantMin={montantMin.Value}";
        }

        if (montantMax.HasValue)
        {
            queryString += $"&montantMax={montantMax.Value}";
        }

        var response = await _httpClient.GetAsync(queryString, cancellationToken: cancellationToken);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<PagedList<PaiementFournisseurResponse>>(responseContent);
        return result!;
    }

    public async Task<Result> UpdatePaiementFournisseurAsync(
        int id,
        UpdatePaiementFournisseurRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating paiement fournisseur via API /paiement-fournisseur/{id}", id);
        var response = await _httpClient.PutAsJsonAsync($"/paiement-fournisseur/{id}", request, cancellationToken: cancellationToken);

        if (response.StatusCode == HttpStatusCode.NoContent)
        {
            return Result.Ok();
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return Result.Fail("paiement_fournisseur_not_found");
        }

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            var badRequest = await response.ReadJsonAsync<BadRequestResponse>();
            return Result.Fail($"validation_error: {JsonConvert.SerializeObject(badRequest)}");
        }

        throw new Exception($"PaiementFournisseur: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
    }

    public async Task<Result> DeletePaiementFournisseurAsync(
        int id,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting paiement fournisseur via API /paiement-fournisseur/{id}", id);
        var response = await _httpClient.DeleteAsync($"/paiement-fournisseur/{id}", cancellationToken: cancellationToken);

        if (response.StatusCode == HttpStatusCode.NoContent)
        {
            return Result.Ok();
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return Result.Fail("paiement_fournisseur_not_found");
        }

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            var badRequest = await response.ReadJsonAsync<BadRequestResponse>();
            return Result.Fail($"validation_error: {JsonConvert.SerializeObject(badRequest)}");
        }

        throw new Exception($"PaiementFournisseur: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
    }
}

