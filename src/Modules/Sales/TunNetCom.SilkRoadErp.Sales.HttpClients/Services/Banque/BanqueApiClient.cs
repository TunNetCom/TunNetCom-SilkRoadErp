using TunNetCom.SilkRoadErp.Sales.Contracts.Banque;
using TunNetCom.SilkRoadErp.Sales.HttpClients;

namespace TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Banque;

public class BanqueApiClient : IBanqueApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<BanqueApiClient> _logger;

    public BanqueApiClient(HttpClient httpClient, ILogger<BanqueApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<OneOf<int, BadRequestResponse>> CreateBanqueAsync(
        CreateBanqueRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating banque via API /banque");
        var response = await _httpClient.PostAsJsonAsync("/banque", request, cancellationToken: cancellationToken);

        if (response.StatusCode == HttpStatusCode.Created)
        {
            if (response.Headers.Location != null)
            {
                var locationPath = response.Headers.Location.ToString();
                var segments = locationPath.Split('/', StringSplitOptions.RemoveEmptyEntries);
                if (segments.Length >= 2 && int.TryParse(segments[segments.Length - 1], out var banqueId))
                {
                    return banqueId;
                }
            }
            throw new Exception($"Banque: Unable to extract banque id from Location header: {response.Headers.Location}");
        }

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            return await response.ReadJsonAsync<BadRequestResponse>();
        }

        throw new Exception($"Banque: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
    }

    public async Task<List<BanqueResponse>> GetBanquesAsync(
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching banques from API /banque");
        var response = await _httpClient.GetAsync("/banque", cancellationToken: cancellationToken);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<List<BanqueResponse>>(responseContent);
        return result ?? new List<BanqueResponse>();
    }
}

