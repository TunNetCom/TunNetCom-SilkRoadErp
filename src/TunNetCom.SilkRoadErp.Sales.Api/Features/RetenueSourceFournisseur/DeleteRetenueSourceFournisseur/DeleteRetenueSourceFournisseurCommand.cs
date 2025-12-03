namespace TunNetCom.SilkRoadErp.Sales.Api.Features.RetenueSourceFournisseur.DeleteRetenueSourceFournisseur;

public record DeleteRetenueSourceFournisseurCommand(int NumFactureFournisseur) : IRequest<Result>;


