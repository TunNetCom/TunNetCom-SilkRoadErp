using TunNetCom.SilkRoadErp.Sales.Contracts.Invoice;
using TunNetCom.SilkRoadErp.Sales.WebApp.Helpers;

namespace TunNetCom.SilkRoadErp.Sales.WebApp.Services.Invoice
{
    public interface IInvoicesApiClient
    {
        Task<OneOf<InvoiceResponse, bool>> GetInvoice(int id, CancellationToken cancellationToken);
        Task<PagedList<InvoiceResponse>> GetInvoices(QueryStringParameters queryParameters, CancellationToken cancellationToken);
        Task<OneOf<CreateInvoiceRequest, BadRequestResponse>> CreateInvoice(CreateInvoiceRequest request, CancellationToken cancellationToken);
    }
}
