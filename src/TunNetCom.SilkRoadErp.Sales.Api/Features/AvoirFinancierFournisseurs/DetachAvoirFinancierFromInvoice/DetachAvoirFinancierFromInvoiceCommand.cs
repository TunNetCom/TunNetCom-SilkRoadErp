namespace TunNetCom.SilkRoadErp.Sales.Api.Features.AvoirFinancierFournisseurs.DetachAvoirFinancierFromInvoice;

public record DetachAvoirFinancierFromInvoiceCommand(int Id) : IRequest<Result>;
