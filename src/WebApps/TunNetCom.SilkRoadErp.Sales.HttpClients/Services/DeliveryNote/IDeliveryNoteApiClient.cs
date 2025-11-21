using FluentResults;
using TunNetCom.SilkRoadErp.Sales.Contracts.DeliveryNote.Requests;
using TunNetCom.SilkRoadErp.Sales.Contracts.DeliveryNote.Responses;

namespace TunNetCom.SilkRoadErp.Sales.HttpClients.Services.DeliveryNote;

public interface IDeliveryNoteApiClient
{
    Task<PagedList<DeliveryNoteResponse>> GetDeliveryNotes(
        int pageNumber,
        int pageSize,
        int? customerId,
        string? searchKeyword,
        bool? isFactured);

    Task<Result<long>> CreateDeliveryNoteAsync(
       CreateDeliveryNoteRequest request,
       CancellationToken cancellationToken);

    Task<List<DeliveryNoteResponse>> GetDeliveryNotesByClientId(
        int clientId);
    Task<List<DeliveryNoteResponse>> GetDeliveryNotesByInvoiceId(
        int invoiceId);

    Task<bool> AttachToInvoiceAsync(
        AttachToInvoiceRequest request,
        CancellationToken cancellationToken);

    Task<List<DeliveryNoteResponse>> GetUninvoicedDeliveryNotesAsync(
        int clientId,
        CancellationToken cancellationToken);

    Task<OneOf<bool, BadRequestResponse>> DetachFromInvoiceAsync(
        DetachFromInvoiceRequest request,
        CancellationToken cancellationToken);

    Task<GetDeliveryNotesWithSummariesResponse> GetDeliveryNotesWithSummariesAsync(
        int? customerId,
        int? invoiceId,
        bool? isInvoiced,
        string? sortOrder,
        string? sortProperty,
        int pageNumber,
        int pageSize,
        string? searchKeyword,
        DateTime? startDate,
        DateTime? EndDate,
        CancellationToken cancellationToken
        );

    Task<DeliveryNoteResponse?> GetDeliveryNoteByNumAsync(
        int num,
        CancellationToken cancellationToken);

    Task<List<DeliveryNoteDetailResponse>> GetDeliveryNotesAsync(
        string productReference,
        CancellationToken cancellationToken = default);

    Task<Result> UpdateDeliveryNoteAsync(
        int num,
        CreateDeliveryNoteRequest request,
        CancellationToken cancellationToken);
}
