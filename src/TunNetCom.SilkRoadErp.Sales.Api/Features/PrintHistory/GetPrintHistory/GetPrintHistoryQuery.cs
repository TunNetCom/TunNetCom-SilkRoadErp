using TunNetCom.SilkRoadErp.Sales.Contracts.PrintHistory;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.PrintHistory.GetPrintHistory;

public record GetPrintHistoryQuery(
    string? DocumentType,
    int? DocumentId,
    DateTime? DateFrom,
    DateTime? DateTo,
    int? UserId,
    string? PrintMode,
    int PageNumber,
    int PageSize) : IRequest<PagedList<PrintHistoryResponse>>;







