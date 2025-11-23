using TunNetCom.SilkRoadErp.Sales.Contracts.Avoirs;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Avoirs.GetFullAvoir;

public record GetFullAvoirQuery(int Num) : IRequest<Result<FullAvoirResponse>>;

