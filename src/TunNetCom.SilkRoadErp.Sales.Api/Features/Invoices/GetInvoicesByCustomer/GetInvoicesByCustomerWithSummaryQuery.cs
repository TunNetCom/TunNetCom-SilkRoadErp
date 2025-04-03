namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Invoices.GetInvoicesByCustomer;

public record GetInvoicesByCustomerWithSummaryQuery(
        int ClientId,
        int PageNumber,
        int PageSize
    ) : IRequest<Result<GetInvoiceListWithSummary>>;
