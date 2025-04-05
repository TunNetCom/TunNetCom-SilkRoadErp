using OneOf.Types;

namespace TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Invoices;

public interface IInvoicesApiClient
{
    Task<OneOf<GetInvoiceListWithSummary, BadRequestResponse>> GetInvoicesByCustomerIdWithSummary(
        int customerId,
        QueryStringParameters queryParameters,
        CancellationToken cancellationToken);
    Task<PagedList<InvoiceResponse>> GetInvoices(
        QueryStringParameters queryParameters,
        CancellationToken cancellationToken);
    Task<OneOf<CreateInvoiceRequest, BadRequestResponse>> CreateInvoice(
        CreateInvoiceRequest request,
        CancellationToken cancellationToken);

    Task<OneOf<IList<InvoiceResponse>, BadRequestResponse>> GetInvoicesByIdsAsync(
      List<int> invoiceIds,
      CancellationToken cancellationToken);
}
