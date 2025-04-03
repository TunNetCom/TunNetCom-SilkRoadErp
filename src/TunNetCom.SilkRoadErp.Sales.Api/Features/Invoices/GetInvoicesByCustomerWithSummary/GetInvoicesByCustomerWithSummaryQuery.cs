namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Invoices.GetInvoicesByCustomerWithSummary;

public record GetInvoicesByCustomerWithSummaryQuery(
        int ClientId,
        int PageNumber,
        int PageSize
    ) : IRequest<Result<GetInvoiceListWithSummary>>;
