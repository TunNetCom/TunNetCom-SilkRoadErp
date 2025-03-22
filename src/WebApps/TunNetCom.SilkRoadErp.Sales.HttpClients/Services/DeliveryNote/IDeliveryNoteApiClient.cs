using TunNetCom.SilkRoadErp.Sales.Contracts.Customers;
using TunNetCom.SilkRoadErp.Sales.Contracts.DeliveryNote;

namespace TunNetCom.SilkRoadErp.Sales.HttpClients.Services.DeliveryNote;

public interface IDeliveryNoteApiClient
{
    Task<PagedList<DeliveryNoteResponse>> GetDeliveryNotes(
        int pageNumber,
        int pageSize,
        int? customerId,
        string? searchKeyword,
        bool? isFactured);

    Task<int> CreateDeliveryNote(
        CreateDeliveryNoteRequest createDeliveryNoteRequest);

    Task<List<DeliveryNoteResponse>> GetDeliveryNotesByClientId(
        int clientId);
    Task<List<DeliveryNoteResponse>> GetDeliveryNotesByInvoiceId(
        int invoiceId);

    Task<bool> AttachToInvoiceAsync(AttachToInvoiceRequest request, CancellationToken cancellationToken);

    Task<List<DeliveryNoteResponse>> GetUninvoicedDeliveryNotesAsync(
        int clientId,
        CancellationToken cancellationToken);

    Task<OneOf<bool, BadRequestResponse>> DetachFromInvoiceAsync(
DetachFromInvoiceRequest request,
CancellationToken cancellationToken);

}
