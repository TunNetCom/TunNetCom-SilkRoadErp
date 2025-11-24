using TunNetCom.SilkRoadErp.Sales.Contracts.PaiementClient;
using TunNetCom.SilkRoadErp.Sales.HttpClients;

namespace TunNetCom.SilkRoadErp.Sales.HttpClients.Services.PaiementClient;

public class PaiementClientApiClient : IPaiementClientApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<PaiementClientApiClient> _logger;

    public PaiementClientApiClient(HttpClient httpClient, ILogger<PaiementClientApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<OneOf<int, BadRequestResponse>> CreatePaiementClientAsync(
        CreatePaiementClientRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating paiement client via API /paiement-client");
        var response = await _httpClient.PostAsJsonAsync("/paiement-client", request, cancellationToken: cancellationToken);

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
            throw new Exception($"PaiementClient: Unable to extract paiement id from Location header: {response.Headers.Location}");
        }

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            return await response.ReadJsonAsync<BadRequestResponse>();
        }

        throw new Exception($"PaiementClient: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
    }

    public async Task<Result<PaiementClientResponse>> GetPaiementClientAsync(
        int id,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching paiement client from API /paiement-client/{id}", id);
        var response = await _httpClient.GetAsync($"/paiement-client/{id}", cancellationToken: cancellationToken);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            var paiement = JsonConvert.DeserializeObject<PaiementClientResponse>(responseContent);
            return Result.Ok(paiement!);
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return Result.Fail("paiement_client_not_found");
        }

        throw new Exception($"PaiementClient: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
    }

    public async Task<PagedList<PaiementClientResponse>> GetPaiementsClientAsync(
        int? clientId,
        int? accountingYearId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching paiements client from API /paiement-client");
        var queryString = $"/paiement-client?pageNumber={pageNumber}&pageSize={pageSize}";

        if (clientId.HasValue)
        {
            queryString += $"&clientId={clientId.Value}";
        }

        if (accountingYearId.HasValue)
        {
            queryString += $"&accountingYearId={accountingYearId.Value}";
        }

        var response = await _httpClient.GetAsync(queryString, cancellationToken: cancellationToken);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<PagedList<PaiementClientResponse>>(responseContent);
        return result!;
    }

    public async Task<Result> UpdatePaiementClientAsync(
        int id,
        UpdatePaiementClientRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating paiement client via API /paiement-client/{id}", id);
        var response = await _httpClient.PutAsJsonAsync($"/paiement-client/{id}", request, cancellationToken: cancellationToken);

        if (response.StatusCode == HttpStatusCode.NoContent)
        {
            return Result.Ok();
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return Result.Fail("paiement_client_not_found");
        }

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            var badRequest = await response.ReadJsonAsync<BadRequestResponse>();
            return Result.Fail($"validation_error: {JsonConvert.SerializeObject(badRequest)}");
        }

        throw new Exception($"PaiementClient: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
    }

    public async Task<Result> DeletePaiementClientAsync(
        int id,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting paiement client via API /paiement-client/{id}", id);
        var response = await _httpClient.DeleteAsync($"/paiement-client/{id}", cancellationToken: cancellationToken);

        if (response.StatusCode == HttpStatusCode.NoContent)
        {
            return Result.Ok();
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return Result.Fail("paiement_client_not_found");
        }

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            var badRequest = await response.ReadJsonAsync<BadRequestResponse>();
            return Result.Fail($"validation_error: {JsonConvert.SerializeObject(badRequest)}");
        }

        throw new Exception($"PaiementClient: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
    }
}

