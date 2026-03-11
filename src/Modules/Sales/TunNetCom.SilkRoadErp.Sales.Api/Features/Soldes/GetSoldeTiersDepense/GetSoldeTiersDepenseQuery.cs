using TunNetCom.SilkRoadErp.Sales.Contracts.Soldes;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Soldes.GetSoldeTiersDepense;

public record GetSoldeTiersDepenseQuery(int TiersDepenseFonctionnementId, int? AccountingYearId = null) : IRequest<Result<SoldeTiersDepenseResponse>>;
