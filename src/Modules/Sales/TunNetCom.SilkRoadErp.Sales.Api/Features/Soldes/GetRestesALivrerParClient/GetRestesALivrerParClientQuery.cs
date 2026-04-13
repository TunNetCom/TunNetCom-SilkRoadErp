using TunNetCom.SilkRoadErp.Sales.Contracts.Soldes;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Soldes.GetRestesALivrerParClient;

public record GetRestesALivrerParClientQuery(int? AccountingYearId = null)
    : IRequest<Result<RestesALivrerParClientResponse>>;
