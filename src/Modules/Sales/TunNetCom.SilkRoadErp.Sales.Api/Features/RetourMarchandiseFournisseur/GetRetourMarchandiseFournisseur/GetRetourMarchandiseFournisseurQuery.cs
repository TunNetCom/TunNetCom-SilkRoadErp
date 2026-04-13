using TunNetCom.SilkRoadErp.Sales.Contracts.RetourMarchandiseFournisseur;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.RetourMarchandiseFournisseur.GetRetourMarchandiseFournisseur;

public record GetRetourMarchandiseFournisseurQuery(int Num) : IRequest<Result<RetourMarchandiseFournisseurResponse>>;

