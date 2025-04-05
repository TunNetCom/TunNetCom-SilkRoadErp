using TunNetCom.SilkRoadErp.Sales.Contracts.AppParameters;

namespace TunNetCom.SilkRoadErp.Sales.HttpClients.Services.AppParameters;

internal class AppParametersClient(HttpClient _httpClient, ILogger<AppParametersClient> _logger) : IAppParametersClient
{
    public async Task<OneOf<GetAppParametersResponse, bool>> GetAppParametersAsync(
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching app parameters from the API /appParameters");

        var response = await _httpClient.GetAsync("/appParameters", cancellationToken);
        if (response.StatusCode == HttpStatusCode.OK)
        {
            var appParameters = await response.Content.ReadFromJsonAsync<GetAppParametersResponse>(cancellationToken);
            return appParameters ?? throw new Exception("Failed to deserialize the response.");
        }
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return false;
        }
        throw new Exception($"AppParameters: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync(cancellationToken)}");
    }
}
