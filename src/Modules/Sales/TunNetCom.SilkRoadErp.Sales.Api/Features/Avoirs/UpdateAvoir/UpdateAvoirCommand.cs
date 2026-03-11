using TunNetCom.SilkRoadErp.Sales.Contracts.Avoirs;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Avoirs.UpdateAvoir;

public record UpdateAvoirCommand(
    int Num,
    DateTime Date,
    int? ClientId,
    List<AvoirLineRequest> Lines
) : IRequest<Result>;

