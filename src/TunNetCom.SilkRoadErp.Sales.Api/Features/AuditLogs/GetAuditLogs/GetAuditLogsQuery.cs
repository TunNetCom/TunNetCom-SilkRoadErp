using TunNetCom.SilkRoadErp.Sales.Contracts.AuditLogs;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.AuditLogs.GetAuditLogs;

public record GetAuditLogsQuery(
    string? EntityName,
    string? EntityId,
    DateTime? DateFrom,
    DateTime? DateTo,
    int? UserId,
    string? Action,
    int PageNumber,
    int PageSize) : IRequest<PagedList<AuditLogResponse>>;

