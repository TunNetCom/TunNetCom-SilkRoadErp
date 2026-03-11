using TunNetCom.SilkRoadErp.Sales.Contracts.DeliveryNote.Responses;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.GetDeliveryNoteByNum;

public record GetDeliveryNoteByNumQuery(int Num) : IRequest<Result<DeliveryNoteResponse>>;