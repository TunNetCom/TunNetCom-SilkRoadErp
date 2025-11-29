using TunNetCom.SilkRoadErp.Sales.Contracts.AuditLogs;

namespace TunNetCom.SilkRoadErp.Sales.HttpClients.Services.AuditLogs;

public interface IAuditLogsClient
{
    Task<PagedList<AuditLogResponse>> GetAuditLogsAsync(
        GetAuditLogsRequest request,
        CancellationToken cancellationToken);

    Task<List<AuditLogDetailResponse>> GetAuditLogsByEntityAsync(
        string entityName,
        string entityId,
        CancellationToken cancellationToken);
}


