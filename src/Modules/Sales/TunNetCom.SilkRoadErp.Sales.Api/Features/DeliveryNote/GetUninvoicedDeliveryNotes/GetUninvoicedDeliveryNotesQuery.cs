using TunNetCom.SilkRoadErp.Sales.Contracts.DeliveryNote.Responses;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.GetUninvoicedDeliveryNotes;

public record GetUninvoicedDeliveryNotesQuery(int CustomerId) : IRequest<Result<List<DeliveryNoteResponse>>>;