namespace TunNetCom.SilkRoadErp.Sales.Api.Features.FactureAvoirFournisseur.DetachFactureAvoirFournisseurFromInvoice;

public record DetachFactureAvoirFournisseurFromInvoiceCommand(
    int FactureFournisseurId,
    List<int> FactureAvoirFournisseurIds
) : IRequest<Result>;


