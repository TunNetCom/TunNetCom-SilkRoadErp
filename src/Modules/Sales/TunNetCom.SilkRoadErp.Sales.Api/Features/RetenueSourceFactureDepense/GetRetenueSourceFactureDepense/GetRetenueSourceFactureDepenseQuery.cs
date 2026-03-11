using TunNetCom.SilkRoadErp.Sales.Contracts.RetenueSourceFactureDepense;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.RetenueSourceFactureDepense.GetRetenueSourceFactureDepense;

public record GetRetenueSourceFactureDepenseQuery(int FactureDepenseId) : IRequest<Result<RetenueSourceFactureDepenseResponse>>;
