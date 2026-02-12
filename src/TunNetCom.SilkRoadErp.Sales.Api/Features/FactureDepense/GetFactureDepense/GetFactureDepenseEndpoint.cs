using Carter;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Constants;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.ResultExtensions;
using TunNetCom.SilkRoadErp.Sales.Contracts.FactureDepense;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.FactureDepense.GetFactureDepense;

public class GetFactureDepenseEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/factures-depenses/{id:int}", async (IMediator mediator, int id, CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new GetFactureDepenseQuery(id), cancellationToken);
            if (result.IsEntityNotFound())
                return Results.NotFound();
            return Results.Ok(result.Value);
        })
        .RequireAuthorization($"Permission:{Permissions.ViewFactureDepense}")
        .WithTags(EndpointTags.FactureDepense);
    }
}
