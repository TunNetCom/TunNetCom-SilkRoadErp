using TunNetCom.SilkRoadErp.Sales.Contracts.DeliveryCar;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryCar.GetAllDeliveryCars;

public record GetAllDeliveryCarsQuery() : IRequest<List<DeliveryCarResponse>>;



