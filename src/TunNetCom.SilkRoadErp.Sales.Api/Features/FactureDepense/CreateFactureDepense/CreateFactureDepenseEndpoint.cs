using Carter;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Constants;
using TunNetCom.SilkRoadErp.Sales.Contracts.FactureDepense;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.FactureDepense.CreateFactureDepense;

public class CreateFactureDepenseEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/factures-depenses", async (IMediator mediator, CreateFactureDepenseRequest request, CancellationToken cancellationToken) =>
        {
            var command = new CreateFactureDepenseCommand(
                request.IdTiersDepenseFonctionnement,
                request.Date,
                request.Description ?? string.Empty,
                request.MontantTotal,
                request.AccountingYearId);
            var result = await mediator.Send(command, cancellationToken);
            if (result.IsFailed)
                return result.IsEntityNotFound() ? Results.NotFound() : Results.BadRequest(result.Errors);
            return Results.Created($"/factures-depenses/{result.Value}", new { id = result.Value });
        })
        .RequireAuthorization($"Permission:{Permissions.CreateFactureDepense}")
        .WithTags(EndpointTags.FactureDepense);
    }
}
