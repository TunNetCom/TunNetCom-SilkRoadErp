using TunNetCom.SilkRoadErp.Sales.Contracts;
using TunNetCom.SilkRoadErp.Sales.Contracts.PaiementTiersDepense;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.PaiementTiersDepense.GetPaiementsTiersDepense;

public record GetPaiementsTiersDepenseQuery(
    int? TiersDepenseFonctionnementId = null,
    int? AccountingYearId = null,
    int PageNumber = 1,
    int PageSize = 10) : IRequest<Result<PagedList<PaiementTiersDepenseResponse>>>;
