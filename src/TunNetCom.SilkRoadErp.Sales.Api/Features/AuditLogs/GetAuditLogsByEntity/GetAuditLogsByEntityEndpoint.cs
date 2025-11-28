using TunNetCom.SilkRoadErp.Sales.Contracts.AuditLogs;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.AuditLogs.GetAuditLogsByEntity;

public class GetAuditLogsByEntityEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/audit-logs/{entityName}/{entityId}", async (
                string entityName,
                string entityId,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                if (string.IsNullOrEmpty(entityName) || string.IsNullOrEmpty(entityId))
                {
                    return Results.Problem(
                        statusCode: StatusCodes.Status400BadRequest,
                        detail: "EntityName and EntityId are required.");
                }

                var query = new GetAuditLogsByEntityQuery(entityName, entityId);
                var auditLogs = await mediator.Send(query, cancellationToken);
                return Results.Ok(auditLogs);
            })
            .Produces<List<AuditLogDetailResponse>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithDescription("Retrieves audit logs for a specific entity.")
            .RequireAuthorization($"Permission:{Permissions.ViewAuditLogs}")
            .WithTags("AuditLogs")
            .WithOpenApi();
    }
}

