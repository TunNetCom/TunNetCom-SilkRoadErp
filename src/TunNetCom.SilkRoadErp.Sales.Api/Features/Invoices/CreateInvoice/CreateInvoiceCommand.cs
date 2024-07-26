namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Invoices.CreateInvoice;

public record CreateInvoiceCommand(
        DateTime Date,
        int ClientId
    ) : IRequest<Result<int>>;
