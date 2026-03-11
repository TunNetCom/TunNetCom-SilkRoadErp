using TunNetCom.SilkRoadErp.Sales.Contracts.Invoice;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Invoices.GetInvoiceTotals;

public record GetInvoiceTotalsQuery(
    DateTime? StartDate = null,
    DateTime? EndDate = null,
    int? CustomerId = null,
    int[]? TagIds = null,
    int? Status = null
) : IRequest<InvoiceTotalsResponse>;
