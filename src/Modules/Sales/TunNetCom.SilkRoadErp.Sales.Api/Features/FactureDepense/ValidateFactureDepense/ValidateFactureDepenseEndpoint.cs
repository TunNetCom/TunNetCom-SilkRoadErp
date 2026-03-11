using Carter;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Constants;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.ResultExtensions;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.FactureDepense.ValidateFactureDepense;

public class ValidateFactureDepenseEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/factures-depenses/{id:int}/validate", async (IMediator mediator, int id, CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new ValidateFactureDepenseCommand(id), cancellationToken);
            if (result.IsFailed)
                return result.IsEntityNotFound() ? Results.NotFound() : Results.BadRequest(result.Errors);
            return Results.NoContent();
        })
        .RequireAuthorization($"Permission:{Permissions.ValidateFactureDepense}")
        .WithTags(EndpointTags.FactureDepense);
    }
}
