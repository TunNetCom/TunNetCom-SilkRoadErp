using TunNetCom.SilkRoadErp.Sales.Contracts;
using TunNetCom.SilkRoadErp.Sales.Contracts.ReceiptNote.Responses;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ReceiptNote.GetReceiptNotesBasedOnProductReference;

public record GetReceiptNotesBasedOnProductReferenceQuery(
    string ProductReference,
    int PageNumber,
    int PageSize) : IRequest<PagedList<ReceiptNoteDetailResponse>>;

