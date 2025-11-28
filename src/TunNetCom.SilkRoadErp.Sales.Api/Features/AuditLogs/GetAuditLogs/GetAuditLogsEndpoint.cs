using TunNetCom.SilkRoadErp.Sales.Contracts.AuditLogs;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.AuditLogs.GetAuditLogs;

public class GetAuditLogsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/audit-logs", async (
                [FromQuery] string? entityName,
                [FromQuery] string? entityId,
                [FromQuery] DateTime? dateFrom,
                [FromQuery] DateTime? dateTo,
                [FromQuery] int? userId,
                [FromQuery] string? action,
                [FromQuery] int pageNumber,
                [FromQuery] int pageSize,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var query = new GetAuditLogsQuery(
                    entityName,
                    entityId,
                    dateFrom,
                    dateTo,
                    userId,
                    action,
                    pageNumber,
                    pageSize);

                var pagedAuditLogs = await mediator.Send(query, cancellationToken);
                return Results.Ok(pagedAuditLogs);
            })
            .Produces<PagedList<AuditLogResponse>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithDescription("Retrieves a paginated and filtered list of audit logs.")
            .RequireAuthorization($"Permission:{Permissions.ViewAuditLogs}")
            .WithTags("AuditLogs")
            .WithOpenApi();
    }
}

