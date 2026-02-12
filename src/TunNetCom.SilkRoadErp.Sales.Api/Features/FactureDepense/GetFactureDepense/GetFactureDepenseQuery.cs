using TunNetCom.SilkRoadErp.Sales.Contracts.FactureDepense;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.FactureDepense.GetFactureDepense;

public record GetFactureDepenseQuery(int Id) : IRequest<Result<FactureDepenseResponse>>;
