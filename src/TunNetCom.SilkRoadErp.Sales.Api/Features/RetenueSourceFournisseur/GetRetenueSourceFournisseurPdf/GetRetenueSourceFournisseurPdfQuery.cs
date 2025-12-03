namespace TunNetCom.SilkRoadErp.Sales.Api.Features.RetenueSourceFournisseur.GetRetenueSourceFournisseurPdf;

public record GetRetenueSourceFournisseurPdfQuery(int NumFactureFournisseur) : IRequest<Result<byte[]>>;


