using System.Text.Json;
using TunNetCom.SilkRoadErp.Sales.Contracts.AuditLogs;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.AuditLogs.GetAuditLogs;

public class GetAuditLogsQueryHandler(
    SalesContext _context,
    ILogger<GetAuditLogsQueryHandler> _logger)
    : IRequestHandler<GetAuditLogsQuery, PagedList<AuditLogResponse>>
{
    public async Task<PagedList<AuditLogResponse>> Handle(GetAuditLogsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching audit logs with filters: EntityName={EntityName}, EntityId={EntityId}, DateFrom={DateFrom}, DateTo={DateTo}, UserId={UserId}, Action={Action}", 
            request.EntityName, request.EntityId, request.DateFrom, request.DateTo, request.UserId, request.Action);

        var query = _context.Set<AuditLog>().AsNoTracking().AsQueryable();

        // Apply filters
        if (!string.IsNullOrEmpty(request.EntityName))
        {
            query = query.Where(a => a.EntityName == request.EntityName);
        }

        if (!string.IsNullOrEmpty(request.EntityId))
        {
            query = query.Where(a => a.EntityId == request.EntityId);
        }

        if (request.DateFrom.HasValue)
        {
            query = query.Where(a => a.Timestamp >= request.DateFrom.Value);
        }

        if (request.DateTo.HasValue)
        {
            query = query.Where(a => a.Timestamp <= request.DateTo.Value);
        }

        if (request.UserId.HasValue)
        {
            query = query.Where(a => a.UserId == request.UserId.Value);
        }

        if (!string.IsNullOrEmpty(request.Action))
        {
            if (Enum.TryParse<AuditAction>(request.Action, true, out var action))
            {
                query = query.Where(a => a.Action == action);
            }
        }

        // Order by timestamp descending (most recent first)
        query = query.OrderByDescending(a => a.Timestamp);

        // Project to response DTO
        var auditLogsQuery = query.Select(a => new AuditLogResponse
        {
            Id = a.Id,
            EntityName = a.EntityName,
            EntityId = a.EntityId,
            Action = a.Action.ToString(),
            UserId = a.UserId,
            Username = a.Username,
            Timestamp = a.Timestamp,
            ChangedProperties = a.ChangedProperties != null 
                ? System.Text.Json.JsonSerializer.Deserialize<List<string>>(a.ChangedProperties) 
                : null
        });

        var pagedAuditLogs = await PagedList<AuditLogResponse>.ToPagedListAsync(
            auditLogsQuery,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        _logger.LogInformation("Retrieved {Count} audit logs", pagedAuditLogs.Items.Count);

        return pagedAuditLogs;
    }
}