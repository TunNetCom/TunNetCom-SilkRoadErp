using TunNetCom.SilkRoadErp.Sales.Contracts.DeliveryCar;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryCar.GetDeliveryCarById;

public record GetDeliveryCarByIdQuery(int Id) : IRequest<Result<DeliveryCarResponse>>;





