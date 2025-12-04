using TunNetCom.SilkRoadErp.Sales.Contracts;
using TunNetCom.SilkRoadErp.Sales.Contracts.Soldes;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Soldes.GetClientsAvecProblemesSolde;

public record GetClientsAvecProblemesSoldeQuery(
    int PageNumber,
    int PageSize,
    int? AccountingYearId = null) : IRequest<PagedList<ClientSoldeProblemeResponse>>;

