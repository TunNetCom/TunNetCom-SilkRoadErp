namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Invoices.GetInvoicesWithIds;

public record GetInvoicesWithIdsQuery(
        List<int> InvoicesIds
    ) : IRequest<Result<List<InvoiceResponse>>>;
