using FluentResults;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Extensions;
using TunNetCom.SilkRoadErp.Sales.Contracts.PrintHistory;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.PrintHistory.CreatePrintHistory;

public class CreatePrintHistoryEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/api/print-history", async (
                CreatePrintHistoryRequest request,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                // Parse PrintMode string to enum
                if (!Enum.TryParse<PrintModeEnum>(request.PrintMode, true, out var printMode))
                {
                    return Results.BadRequest(new { Error = $"Invalid PrintMode: {request.PrintMode}" });
                }

                var command = new CreatePrintHistoryCommand(
                    DocumentType: request.DocumentType,
                    DocumentId: request.DocumentId,
                    PrintMode: printMode,
                    UserId: request.UserId,
                    Username: request.Username,
                    PrinterName: request.PrinterName,
                    Copies: request.Copies,
                    FileName: request.FileName,
                    IsDuplicata: request.IsDuplicata);

                var result = await mediator.Send(command, cancellationToken);
                if (result.IsSuccess)
                {
                    return Results.Ok(result.Value);
                }
                return result.ToValidationProblem();
            })
            .WithTags("PrintHistory")
            .RequireAuthorization()
            .WithOpenApi(operation => new(operation)
            {
                Summary = "Creates a new print history record.",
                Description = "Logs a print operation for a document."
            });
    }
}

