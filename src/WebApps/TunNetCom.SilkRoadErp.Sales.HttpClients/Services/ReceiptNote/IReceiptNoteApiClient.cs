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
    //Task<PagedList<ReceiptNoteDetailsResponse>> GetReceiptNotesAsync(
    //   int? idFournisseur,
    //   int pageNumber,
    //   int pageSize,
    //   string? searchKeyword,
    //   bool? isFactured,
    //   CancellationToken cancellationToken);
}
