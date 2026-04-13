namespace TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Tiers;

public interface ITiersApiClient
{
    Task<byte[]> ExportTiersToTxtAsync(string? type = null, CancellationToken cancellationToken = default);
}
