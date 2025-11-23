using TunNetCom.SilkRoadErp.Sales.Contracts.Avoirs;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Avoirs.GetAvoirsWithSummaries;

public record GetAvoirsWithSummariesQuery(
    int PageNumber,
    int PageSize,
    int? ClientId,
    string? SortOrder,
    string? SortProperty,
    string? SearchKeyword,
    DateTime? StartDate,
    DateTime? EndDate
) : IRequest<GetAvoirsWithSummariesResponse>;

