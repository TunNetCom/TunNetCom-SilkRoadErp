using TunNetCom.SilkRoadErp.Sales.Contracts.RecieptNotes;

namespace TunNetCom.SilkRoadErp.Sales.HttpClients.Services.ReceiptNote;

public interface IReceiptNoteApiClient
{
    Task<OneOf<PagedList<ReceiptNoteDetailsResponse>, BadRequestResponse>> GetReceiptNote(
        int providerId,
        QueryStringParameters queryParameters,
        CancellationToken cancellationToken);
    Task<PagedList<ReceiptNoteDetailsResponse>> GetReceiptNotesAsync(
       int? idFournisseur,
       int pageNumber,
       int pageSize,
       string? searchKeyword,
       bool? isFactured,
       CancellationToken cancellationToken);
}
