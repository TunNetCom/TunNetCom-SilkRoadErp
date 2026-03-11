using TunNetCom.SilkRoadErp.Sales.Contracts.RetenueSourceFournisseur;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.RetenueSourceFournisseur.GetRetenueSourceFournisseur;

public record GetRetenueSourceFournisseurQuery(int NumFactureFournisseur) : IRequest<Result<RetenueSourceFournisseurResponse>>;


