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

    Task DetachReceiptNotesFromInvoiceAsync(
        int invoiceId,
        List<int> receiptNotesIds,
        CancellationToken cancellationToken = default);
}
