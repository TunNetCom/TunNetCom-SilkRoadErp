using TunNetCom.SilkRoadErp.Sales.Contracts.PaiementClient;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.PaiementClient.GetPaiementClient;

public record GetPaiementClientQuery(int Id) : IRequest<Result<PaiementClientResponse>>;

