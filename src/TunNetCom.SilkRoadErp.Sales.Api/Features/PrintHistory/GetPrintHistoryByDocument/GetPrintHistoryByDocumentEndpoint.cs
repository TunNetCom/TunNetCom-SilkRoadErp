using Microsoft.AspNetCore.Mvc;
using TunNetCom.SilkRoadErp.Sales.Contracts.PrintHistory;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.PrintHistory.GetPrintHistoryByDocument;

public class GetPrintHistoryByDocumentEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/api/print-history/{documentType}/{documentId}", async (
                [FromRoute] string documentType,
                [FromRoute] int documentId,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var query = new GetPrintHistoryByDocumentQuery(documentType, documentId);
                var printHistory = await mediator.Send(query, cancellationToken);
                return Results.Ok(printHistory);
            })
            .Produces<List<PrintHistoryResponse>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithDescription("Retrieves print history for a specific document.")
            .RequireAuthorization()
            .WithTags("PrintHistory")
            .WithOpenApi();
    }
}









