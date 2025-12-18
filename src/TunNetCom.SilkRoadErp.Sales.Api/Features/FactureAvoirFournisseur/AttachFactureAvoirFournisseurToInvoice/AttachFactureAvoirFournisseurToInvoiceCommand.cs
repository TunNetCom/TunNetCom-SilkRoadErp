namespace TunNetCom.SilkRoadErp.Sales.Api.Features.FactureAvoirFournisseur.AttachFactureAvoirFournisseurToInvoice;

public record AttachFactureAvoirFournisseurToInvoiceCommand(
    List<int> FactureAvoirFournisseurIds,
    int FactureFournisseurId
) : IRequest<Result>;







