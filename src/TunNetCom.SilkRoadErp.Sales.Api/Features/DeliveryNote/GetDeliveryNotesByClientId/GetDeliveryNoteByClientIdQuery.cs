namespace TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.GetDeliveryNotesByClientId;

public record GetDeliveryNoteByClientIdQuery(int ClientId) : IRequest<Result<List<DeliveryNoteResponse>>>;