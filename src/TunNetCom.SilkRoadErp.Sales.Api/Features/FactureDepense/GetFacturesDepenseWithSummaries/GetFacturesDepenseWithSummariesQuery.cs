using TunNetCom.SilkRoadErp.Sales.Contracts.FactureDepense;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.FactureDepense.GetFacturesDepenseWithSummaries;

public record GetFacturesDepenseWithSummariesQuery(
    int PageNumber,
    int PageSize,
    int? TiersDepenseFonctionnementId,
    int? AccountingYearId,
    string? SearchKeyword,
    DateTime? StartDate = null,
    DateTime? EndDate = null) : IRequest<GetFacturesDepenseWithSummariesResponse>;
