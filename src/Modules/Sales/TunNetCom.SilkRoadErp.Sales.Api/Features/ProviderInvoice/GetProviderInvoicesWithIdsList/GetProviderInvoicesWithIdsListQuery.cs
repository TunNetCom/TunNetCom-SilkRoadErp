namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ProviderInvoice.GetProviderInvoicesWithIdsList;

public record GetProviderInvoicesWithIdsListQuery (List<int> InvoicesIds) : IRequest<List<ProviderInvoiceResponse>>;
