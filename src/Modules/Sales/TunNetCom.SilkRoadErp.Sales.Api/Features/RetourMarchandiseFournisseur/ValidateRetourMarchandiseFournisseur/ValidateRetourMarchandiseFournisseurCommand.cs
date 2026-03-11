namespace TunNetCom.SilkRoadErp.Sales.Api.Features.RetourMarchandiseFournisseur.ValidateRetourMarchandiseFournisseur;

public record ValidateRetourMarchandiseFournisseurCommand(List<int> Ids) : IRequest<Result>;

