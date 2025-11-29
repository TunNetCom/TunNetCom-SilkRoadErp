using TunNetCom.SilkRoadErp.Sales.Api.Features.ReceiptNote.CreateReceiptNote;
using TunNetCom.SilkRoadErp.Sales.Contracts.ReceiptNoteLine.Request;
using TunNetCom.SilkRoadErp.Sales.Contracts.ReceiptNoteLine.Response;
using TunNetCom.SilkRoadErp.Sales.Contracts.ReceiptNote.Responses;

namespace TunNetCom.SilkRoadErp.Sales.HttpClients.Services.ReceiptNote;

public interface IReceiptNoteApiClient
{
    Task<Result<ReceiptNotesWithSummaryResponse>> GetReceiptNoteWithSummaries(
        int? providerId,
        bool? IsInvoiced,
        int? InvoiceId,
        QueryStringParameters queryParameters,
        CancellationToken cancellationToken);

    Task <Result<ReceiptNotesResponse>> GetReceiptNotes(
        int PageNumber,
        string SearchKeyword,
        int PageSize,
        string SortProprety,
        string SortOrder,
        CancellationToken cancellationToken);

    Task<bool> AttachReceiptNotesToInvoiceAsync(
        int invoiceId,
        List<int> receiptNotesIds,
        CancellationToken cancellationToken = default);

    Task<bool> DetachReceiptNotesFromInvoiceAsync(
        int invoiceId,
        List<int> receiptNotesIds,
        CancellationToken cancellationToken = default);
    Task<Result<long>> CreateReceiptNote(
        CreateReceiptNoteRequest request,
        CancellationToken cancellationToken = default);
    Task<Result<List<int>>> CreateReceiptNoteLines(
        List<CreateReceiptNoteLineRequest> request,
        CancellationToken cancellationToken = default);

    Task<Result<ReceiptNoteResponse>> GetReceiptNoteById(
        int id,
        CancellationToken cancellationToken = default);

    Task<Result<GetReceiptNoteLinesByReceiptNoteIdResponse>> GetReceiptNoteLines(
        int id,
        GetReceiptNoteLinesWithSummariesQueryParams queryParams,
        CancellationToken cancellationToken = default);

    Task<Result<int>> CreateReceiptNoteWithLinesRequestTemplate(CreateReceiptNoteWithLinesRequest createReceiptNoteWithLinesRequest,
        CancellationToken cancellationToken = default);

    Task<PagedList<ReceiptNoteDetailResponse>> GetReceiptNotesByProductReferenceAsync(
        string productReference,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<Result> UpdateReceiptNoteAsync(
        int num,
        UpdateReceiptNoteRequest request,
        CancellationToken cancellationToken = default);

    Task<Result> UpdateReceiptNoteWithLinesAsync(
        int num,
        CreateReceiptNoteWithLinesRequest request,
        CancellationToken cancellationToken = default);

    Task<Result> ValidateReceiptNotesAsync(List<int> ids, CancellationToken cancellationToken = default);
}
