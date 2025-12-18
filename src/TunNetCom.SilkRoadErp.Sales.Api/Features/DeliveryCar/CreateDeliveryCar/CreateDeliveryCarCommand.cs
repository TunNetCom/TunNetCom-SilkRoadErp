using TunNetCom.SilkRoadErp.Sales.Contracts.DeliveryCar;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryCar.CreateDeliveryCar;

public record CreateDeliveryCarCommand(
    string Matricule,
    string Owner
) : IRequest<Result<DeliveryCarResponse>>;




