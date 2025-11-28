using TunNetCom.SilkRoadErp.Sales.Contracts.AuditLogs;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.AuditLogs.GetAuditLogsByEntity;

public record GetAuditLogsByEntityQuery(
    string EntityName,
    string EntityId) : IRequest<List<AuditLogDetailResponse>>;

