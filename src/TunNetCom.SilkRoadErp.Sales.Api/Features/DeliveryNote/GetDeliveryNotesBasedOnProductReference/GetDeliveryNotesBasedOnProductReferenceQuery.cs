using TunNetCom.SilkRoadErp.Sales.Contracts.DeliveryNote.Responses;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.GetDeliveryNotesBasedOnProductReference;

public record GetDeliveryNotesBasedOnProductReferenceQuery(
    string ProductReference,
    int PageNumber,
    int PageSize) : IRequest<PagedList<DeliveryNoteDetailResponse>>;
