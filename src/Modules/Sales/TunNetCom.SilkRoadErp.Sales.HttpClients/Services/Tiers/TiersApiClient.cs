using System.Net.Http.Json;

namespace TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Tiers;

public class TiersApiClient(HttpClient httpClient) : ITiersApiClient
{
    public async Task<byte[]> ExportTiersToTxtAsync(string? type = null, CancellationToken cancellationToken = default)
    {
        var url = string.IsNullOrEmpty(type) ? "/api/tiers/export/txt" : $"/api/tiers/export/txt?type={type}";
        var response = await httpClient.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsByteArrayAsync(cancellationToken);
    }
}
