using TunNetCom.SilkRoadErp.Sales.Contracts.DeliveryCar;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryCar.UpdateDeliveryCar;

public record UpdateDeliveryCarCommand(
    int Id,
    string Matricule,
    string Owner
) : IRequest<Result<DeliveryCarResponse>>;



