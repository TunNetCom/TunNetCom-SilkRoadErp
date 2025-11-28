using System.Text.Json;
using TunNetCom.SilkRoadErp.Sales.Contracts.AuditLogs;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.AuditLogs.GetAuditLogsByEntity;

public class GetAuditLogsByEntityQueryHandler(
    SalesContext _context,
    ILogger<GetAuditLogsByEntityQueryHandler> _logger)
    : IRequestHandler<GetAuditLogsByEntityQuery, List<AuditLogDetailResponse>>
{
    public async Task<List<AuditLogDetailResponse>> Handle(GetAuditLogsByEntityQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching audit logs for entity: {EntityName} with ID: {EntityId}", 
            request.EntityName, request.EntityId);

        var auditLogs = await _context.Set<AuditLog>()
            .AsNoTracking()
            .Where(a => a.EntityName == request.EntityName && a.EntityId == request.EntityId)
            .OrderByDescending(a => a.Timestamp)
            .ToListAsync(cancellationToken);

        var response = auditLogs.Select(a => new AuditLogDetailResponse
        {
            Id = a.Id,
            EntityName = a.EntityName,
            EntityId = a.EntityId,
            Action = a.Action.ToString(),
            UserId = a.UserId,
            Username = a.Username,
            Timestamp = a.Timestamp,
            OldValues = a.OldValues != null 
                ? System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object?>>(a.OldValues) 
                : null,
            NewValues = a.NewValues != null 
                ? System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object?>>(a.NewValues) 
                : null,
            ChangedProperties = a.ChangedProperties != null 
                ? System.Text.Json.JsonSerializer.Deserialize<List<string>>(a.ChangedProperties) 
                : null
        }).ToList();

        _logger.LogInformation("Retrieved {Count} audit logs for entity {EntityName}:{EntityId}", 
            response.Count, request.EntityName, request.EntityId);

        return response;
    }
}

