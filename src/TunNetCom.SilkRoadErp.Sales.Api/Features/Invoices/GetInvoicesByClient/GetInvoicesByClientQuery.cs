namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Invoices.GetInvoicesByClient;

public record GetInvoicesByClientQuery(
        int ClientId,
        int PageNumber,
        int PageSize
    ) : IRequest<Result<PagedList<InvoiceResponse>>>;
