namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Invoices.GetFullInvoiceById;

public record GetFullInvoiceByIdQuery(int Id) : IRequest<Result<FullInvoiceResponse>>;
