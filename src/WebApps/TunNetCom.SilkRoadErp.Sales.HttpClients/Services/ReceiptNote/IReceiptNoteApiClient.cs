using TunNetCom.SilkRoadErp.Sales.Contracts.RecieptNotes;

namespace TunNetCom.SilkRoadErp.Sales.HttpClients.Services.ReceiptNote;

public interface IReceiptNoteApiClient
{
    Task<ReceiptNotesWithSummary> GetReceiptNote(
        int providerId,
        bool IsInvoiced,
        int? InvoiceId,
        QueryStringParameters queryParameters,
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
