using TunNetCom.SilkRoadErp.Sales.Contracts.Soldes;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Soldes.GetSoldeClient;

public record GetSoldeClientQuery(int ClientId, int? AccountingYearId = null) : IRequest<Result<SoldeClientResponse>>;

