using System.Net.Http.Json;
using TunNetCom.SilkRoadErp.Sales.Contracts.Dashboard;

namespace TunNetCom.SilkRoadErp.Sales.WebApp.Services.Dashboard;

public class DashboardEvolutionService : IDashboardEvolutionService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<DashboardEvolutionService> _logger;

    public DashboardEvolutionService(HttpClient httpClient, ILogger<DashboardEvolutionService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<EvolutionVentesAchatsResponse?> GetEvolutionAsync(int months = 12, CancellationToken cancellationToken = default)
    {
        try
        {
            var url = $"/api/dashboard/evolution?months={Math.Clamp(months, 1, 24)}";
            var response = await _httpClient.GetAsync(url, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Dashboard evolution API returned {StatusCode}", response.StatusCode);
                return null;
            }

            return await response.Content.ReadFromJsonAsync<EvolutionVentesAchatsResponse>(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading dashboard evolution");
            return null;
        }
    }
}
