using TunNetCom.SilkRoadErp.Sales.Contracts.RetenueSourceClient;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.RetenueSourceClient.GetRetenueSourceClient;

public record GetRetenueSourceClientQuery(int NumFacture) : IRequest<Result<RetenueSourceClientResponse>>;


