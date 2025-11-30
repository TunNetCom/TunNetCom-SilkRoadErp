using TunNetCom.SilkRoadErp.Sales.Contracts.Banque;

namespace TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Banque;

public interface IBanqueApiClient
{
    Task<OneOf<int, BadRequestResponse>> CreateBanqueAsync(
        CreateBanqueRequest request,
        CancellationToken cancellationToken);

    Task<List<BanqueResponse>> GetBanquesAsync(
        CancellationToken cancellationToken);
}