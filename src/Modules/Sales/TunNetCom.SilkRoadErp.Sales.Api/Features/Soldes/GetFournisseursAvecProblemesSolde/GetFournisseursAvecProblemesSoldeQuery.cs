using TunNetCom.SilkRoadErp.Sales.Contracts;
using TunNetCom.SilkRoadErp.Sales.Contracts.Soldes;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Soldes.GetFournisseursAvecProblemesSolde;

public record GetFournisseursAvecProblemesSoldeQuery(
    int PageNumber,
    int PageSize,
    int? AccountingYearId = null) : IRequest<PagedList<FournisseurSoldeProblemeResponse>>;
