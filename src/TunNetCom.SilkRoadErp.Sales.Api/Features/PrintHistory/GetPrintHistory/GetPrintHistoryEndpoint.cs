using Microsoft.AspNetCore.Mvc;
using TunNetCom.SilkRoadErp.Sales.Contracts.PrintHistory;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.PrintHistory.GetPrintHistory;

public class GetPrintHistoryEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/api/print-history", async (
                [FromQuery] string? documentType,
                [FromQuery] int? documentId,
                [FromQuery] DateTime? dateFrom,
                [FromQuery] DateTime? dateTo,
                [FromQuery] int? userId,
                [FromQuery] string? printMode,
                [FromQuery] int pageNumber,
                [FromQuery] int pageSize,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var query = new GetPrintHistoryQuery(
                    documentType,
                    documentId,
                    dateFrom,
                    dateTo,
                    userId,
                    printMode,
                    pageNumber,
                    pageSize);

                var pagedPrintHistory = await mediator.Send(query, cancellationToken);
                return Results.Ok(pagedPrintHistory);
            })
            .Produces<PagedList<PrintHistoryResponse>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithDescription("Retrieves a paginated and filtered list of print history records.")
            .RequireAuthorization()
            .WithTags("PrintHistory")
            .WithOpenApi();
    }
}






