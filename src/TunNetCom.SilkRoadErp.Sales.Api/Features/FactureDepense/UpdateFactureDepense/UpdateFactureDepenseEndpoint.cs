using Carter;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Constants;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.ResultExtensions;
using TunNetCom.SilkRoadErp.Sales.Contracts.FactureDepense;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.FactureDepense.UpdateFactureDepense;

public class UpdateFactureDepenseEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPut("/factures-depenses/{id:int}", async (IMediator mediator, int id, UpdateFactureDepenseRequest request, CancellationToken cancellationToken) =>
        {
            var command = new UpdateFactureDepenseCommand(id, request.Date, request.Description ?? string.Empty, request.MontantTotal, request.DocumentBase64);
            var result = await mediator.Send(command, cancellationToken);
            if (result.IsFailed)
                return result.IsEntityNotFound() ? Results.NotFound() : Results.BadRequest(result.Errors);
            return Results.NoContent();
        })
        .RequireAuthorization($"Permission:{Permissions.UpdateFactureDepense}")
        .WithTags(EndpointTags.FactureDepense);
    }
}
