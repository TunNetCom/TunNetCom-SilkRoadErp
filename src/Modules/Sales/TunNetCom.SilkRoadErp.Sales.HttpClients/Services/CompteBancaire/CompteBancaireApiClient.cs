using TunNetCom.SilkRoadErp.Sales.Contracts.CompteBancaire;
using TunNetCom.SilkRoadErp.Sales.HttpClients;

namespace TunNetCom.SilkRoadErp.Sales.HttpClients.Services.CompteBancaire;

public class CompteBancaireApiClient : ICompteBancaireApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<CompteBancaireApiClient> _logger;

    public CompteBancaireApiClient(HttpClient httpClient, ILogger<CompteBancaireApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<OneOf<int, BadRequestResponse>> CreateCompteBancaireAsync(
        CreateCompteBancaireRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating compte bancaire via API /compte-bancaire");
        var response = await _httpClient.PostAsJsonAsync("/compte-bancaire", request, cancellationToken: cancellationToken);

        if (response.StatusCode == HttpStatusCode.Created)
        {
            if (response.Headers.Location != null)
            {
                var locationPath = response.Headers.Location.ToString();
                var segments = locationPath.Split('/', StringSplitOptions.RemoveEmptyEntries);
                if (segments.Length >= 2 && int.TryParse(segments[segments.Length - 1], out var id))
                {
                    return id;
                }
            }
            throw new Exception("CompteBancaire: Unable to extract id from Location header");
        }

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            return await response.ReadJsonAsync<BadRequestResponse>();
        }

        throw new Exception($"CompteBancaire: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
    }

    public async Task<List<CompteBancaireResponse>> GetCompteBancairesAsync(
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching comptes bancaires from API /compte-bancaire");
        var response = await _httpClient.GetAsync("/compte-bancaire", cancellationToken: cancellationToken);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<List<CompteBancaireResponse>>(responseContent);
        return result ?? new List<CompteBancaireResponse>();
    }

    public async Task<CompteBancaireResponse?> GetCompteBancaireByIdAsync(
        int id,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching compte bancaire {Id} from API", id);
        var response = await _httpClient.GetAsync($"/compte-bancaire/{id}", cancellationToken: cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<CompteBancaireResponse>(responseContent);
    }

    public async Task<OneOf<bool, BadRequestResponse>> UpdateCompteBancaireAsync(
        int id,
        UpdateCompteBancaireRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating compte bancaire {Id} via API", id);
        var response = await _httpClient.PutAsJsonAsync($"/compte-bancaire/{id}", request, cancellationToken: cancellationToken);

        if (response.StatusCode == HttpStatusCode.NoContent)
        {
            return true;
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return await response.ReadJsonAsync<BadRequestResponse>();
        }

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            return await response.ReadJsonAsync<BadRequestResponse>();
        }

        throw new Exception($"CompteBancaire: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
    }

    public async Task<OneOf<bool, BadRequestResponse>> DeleteCompteBancaireAsync(
        int id,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting compte bancaire {Id} via API", id);
        var response = await _httpClient.DeleteAsync($"/compte-bancaire/{id}", cancellationToken: cancellationToken);

        if (response.StatusCode == HttpStatusCode.NoContent)
        {
            return true;
        }

        if (response.StatusCode == HttpStatusCode.NotFound || response.StatusCode == HttpStatusCode.BadRequest)
        {
            return await response.ReadJsonAsync<BadRequestResponse>();
        }

        throw new Exception($"CompteBancaire: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
    }
}
