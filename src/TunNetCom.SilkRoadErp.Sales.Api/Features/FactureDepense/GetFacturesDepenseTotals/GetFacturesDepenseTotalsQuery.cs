using TunNetCom.SilkRoadErp.Sales.Contracts.FactureDepense;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.FactureDepense.GetFacturesDepenseTotals;

public record GetFacturesDepenseTotalsQuery(
    int? AccountingYearId = null,
    DateTime? StartDate = null,
    DateTime? EndDate = null,
    int? TiersDepenseFonctionnementId = null) : IRequest<FactureDepenseTotalsResponse>;
