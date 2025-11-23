using TunNetCom.SilkRoadErp.Sales.Contracts.Avoirs;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Avoirs.CreateAvoir;

public record CreateAvoirCommand(
    DateTime Date,
    int? ClientId,
    List<AvoirLineRequest> Lines
) : IRequest<Result<int>>;

