using TunNetCom.SilkRoadErp.Sales.Contracts.PaiementTiersDepense;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.PaiementTiersDepense.GetPaiementTiersDepense;

public record GetPaiementTiersDepenseQuery(int Id) : IRequest<Result<PaiementTiersDepenseResponse>>;
