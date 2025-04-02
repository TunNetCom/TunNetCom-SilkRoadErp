namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Invoices.GetInvoicesByClient;

public record GetInvoicesByClientWithSummaryQuery(
        int ClientId,
        int PageNumber,
        int PageSize
    ) : IRequest<Result<GetInvoiceListWithSummary>>;
