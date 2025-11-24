using TunNetCom.SilkRoadErp.Sales.Contracts.PaiementClient;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.PaiementClient.GetPaiementsClient;

public record GetPaiementsClientQuery(
    int? ClientId = null,
    int? AccountingYearId = null,
    int PageNumber = 1,
    int PageSize = 10
) : IRequest<Result<PagedList<PaiementClientResponse>>>;

