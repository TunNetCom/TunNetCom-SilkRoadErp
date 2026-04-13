namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ReceiptNote.GetReceiptNote;

public record GetReceiptNoteQuery
(
    int PageNumber,
    int PageSize,
    string? SearchKeyword) : IRequest<PagedList<ReceiptNoteResponse>>;

