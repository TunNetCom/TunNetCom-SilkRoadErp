using TunNetCom.SilkRoadErp.Sales.Contracts.PaiementFournisseur;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.PaiementFournisseur.GetPaiementFournisseur;

public record GetPaiementFournisseurQuery(int Id) : IRequest<Result<PaiementFournisseurResponse>>;

