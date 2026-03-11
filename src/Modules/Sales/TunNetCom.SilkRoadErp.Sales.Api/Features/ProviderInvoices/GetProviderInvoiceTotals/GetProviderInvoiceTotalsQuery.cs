using TunNetCom.SilkRoadErp.Sales.Contracts.ProviderInvoice;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ProviderInvoices.GetProviderInvoiceTotals;

public record GetProviderInvoiceTotalsQuery(
    DateTime? StartDate = null,
    DateTime? EndDate = null,
    int? ProviderId = null,
    int[]? TagIds = null,
    int? Status = null
) : IRequest<ProviderInvoiceTotalsResponse>;
