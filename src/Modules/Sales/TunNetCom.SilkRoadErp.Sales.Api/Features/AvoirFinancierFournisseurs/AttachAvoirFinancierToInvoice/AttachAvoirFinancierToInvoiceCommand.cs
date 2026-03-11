using TunNetCom.SilkRoadErp.Sales.Contracts.AvoirFinancierFournisseurs;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.AvoirFinancierFournisseurs.AttachAvoirFinancierToInvoice;

public record AttachAvoirFinancierToInvoiceCommand(int Id, int NumFactureFournisseur) : IRequest<Result>;
