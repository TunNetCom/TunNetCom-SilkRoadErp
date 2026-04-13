using TunNetCom.SilkRoadErp.Sales.Contracts.Avoirs;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Avoirs.GetAvoir;

public record GetAvoirQuery(int Num) : IRequest<Result<AvoirResponse>>;

