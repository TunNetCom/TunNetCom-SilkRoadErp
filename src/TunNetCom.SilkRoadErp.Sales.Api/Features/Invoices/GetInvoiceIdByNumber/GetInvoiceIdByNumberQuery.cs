namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Invoices.GetInvoiceIdByNumber;

public record GetInvoiceIdByNumberQuery(int Number) : IRequest<Result<int>>;
