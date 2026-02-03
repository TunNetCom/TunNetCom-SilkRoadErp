using TunNetCom.SilkRoadErp.Sales.Contracts;
using TunNetCom.SilkRoadErp.Sales.Contracts.Soldes;
using TunNetCom.SilkRoadErp.Sales.HttpClients;

namespace TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Soldes;

public class SoldesApiClient : ISoldesApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<SoldesApiClient> _logger;

    public SoldesApiClient(HttpClient httpClient, ILogger<SoldesApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<Result<SoldeClientResponse>> GetSoldeClientAsync(
        int clientId,
        int? accountingYearId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching solde client from API /soldes/client/{clientId}", clientId);
        var queryString = $"/soldes/client/{clientId}";
        
        if (accountingYearId.HasValue)
        {
            queryString += $"?accountingYearId={accountingYearId.Value}";
        }

        var response = await _httpClient.GetAsync(queryString, cancellationToken: cancellationToken);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            var solde = JsonConvert.DeserializeObject<SoldeClientResponse>(responseContent);
            return Result.Ok(solde!);
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return Result.Fail("solde_client_not_found");
        }

        throw new Exception($"Soldes: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
    }

    public async Task<Result<SoldeFournisseurResponse>> GetSoldeFournisseurAsync(
        int fournisseurId,
        int? accountingYearId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching solde fournisseur from API /soldes/fournisseur/{fournisseurId}", fournisseurId);
        var queryString = $"/soldes/fournisseur/{fournisseurId}";
        
        if (accountingYearId.HasValue)
        {
            queryString += $"?accountingYearId={accountingYearId.Value}";
        }

        var response = await _httpClient.GetAsync(queryString, cancellationToken: cancellationToken);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            var solde = JsonConvert.DeserializeObject<SoldeFournisseurResponse>(responseContent);
            return Result.Ok(solde!);
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return Result.Fail("solde_fournisseur_not_found");
        }

        throw new Exception($"Soldes: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
    }

    public async Task<PagedList<ClientSoldeProblemeResponse>> GetClientsAvecProblemesSoldeAsync(
        int pageNumber,
        int pageSize,
        int? accountingYearId = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching clients avec problemes solde from API /soldes/clients-avec-problemes");
        
        var queryString = $"/soldes/clients-avec-problemes?pageNumber={pageNumber}&pageSize={pageSize}";
        
        if (accountingYearId.HasValue)
        {
            queryString += $"&accountingYearId={accountingYearId.Value}";
        }

        try
        {
            var response = await _httpClient.GetAsync(queryString, cancellationToken: cancellationToken);
            response.EnsureSuccessStatusCode();
            
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = JsonConvert.DeserializeObject<PagedList<ClientSoldeProblemeResponse>>(responseContent);
            return result ?? new PagedList<ClientSoldeProblemeResponse>(new List<ClientSoldeProblemeResponse>(), 0, pageNumber, pageSize);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching clients avec problemes solde");
            throw;
        }
    }

    public async Task<(byte[] Content, string FileName)> ExportClientsAvecProblemesSoldeToPdfAsync(
        int? accountingYearId = null,
        CancellationToken cancellationToken = default)
    {
        var queryString = accountingYearId.HasValue ? $"?accountingYearId={accountingYearId.Value}" : string.Empty;
        var requestUri = $"/soldes/clients-avec-problemes/export/pdf{queryString}";
        var response = await _httpClient.GetAsync(requestUri, cancellationToken);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsByteArrayAsync(cancellationToken);
        var fileName = response.Content.Headers.ContentDisposition?.FileName?.Trim('"') ?? $"ClientsProblemesSolde_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
        return (content, fileName);
    }

    public async Task<(byte[] Content, string FileName)> ExportClientsAvecProblemesSoldeToExcelAsync(
        int? accountingYearId = null,
        CancellationToken cancellationToken = default)
    {
        var queryString = accountingYearId.HasValue ? $"?accountingYearId={accountingYearId.Value}" : string.Empty;
        var requestUri = $"/soldes/clients-avec-problemes/export/excel{queryString}";
        var response = await _httpClient.GetAsync(requestUri, cancellationToken);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsByteArrayAsync(cancellationToken);
        var fileName = response.Content.Headers.ContentDisposition?.FileName?.Trim('"') ?? $"ClientsProblemesSolde_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
        return (content, fileName);
    }

    public async Task<Result<RestesALivrerParClientResponse>> GetRestesALivrerParClientAsync(
        int? accountingYearId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching restes à livrer par client from API /soldes/clients-avec-problemes/restes-a-livrer");
        var queryString = "/soldes/clients-avec-problemes/restes-a-livrer";
        if (accountingYearId.HasValue)
        {
            queryString += $"?accountingYearId={accountingYearId.Value}";
        }

        var response = await _httpClient.GetAsync(queryString, cancellationToken);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = JsonConvert.DeserializeObject<RestesALivrerParClientResponse>(responseContent);
            return Result.Ok(result!);
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return Result.Fail("restes_a_livrer_not_found");
        }

        throw new Exception($"Restes à livrer: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync(cancellationToken)}");
    }

    public async Task<Result<SoldeTiersDepenseResponse>> GetSoldeTiersDepenseAsync(
        int tiersDepenseFonctionnementId,
        int? accountingYearId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching solde tiers dépense from API /soldes/tiers-depenses/{TiersDepenseFonctionnementId}", tiersDepenseFonctionnementId);
        var queryString = $"/soldes/tiers-depenses/{tiersDepenseFonctionnementId}";

        if (accountingYearId.HasValue)
        {
            queryString += $"?accountingYearId={accountingYearId.Value}";
        }

        var response = await _httpClient.GetAsync(queryString, cancellationToken: cancellationToken);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            var solde = JsonConvert.DeserializeObject<SoldeTiersDepenseResponse>(responseContent);
            return Result.Ok(solde!);
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return Result.Fail("solde_tiers_depense_not_found");
        }

        throw new Exception($"Soldes tiers dépense: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
    }
}

