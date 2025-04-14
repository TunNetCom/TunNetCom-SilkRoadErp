namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ProviderInvoice.GetFullProviderInvoice;

public record GetFullProviderInvoiceQuery(int Id) : IRequest<Result<FullProviderInvoiceResponse>>;
