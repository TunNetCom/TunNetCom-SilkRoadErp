using Carter;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Constants;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.ResultExtensions;
using TunNetCom.SilkRoadErp.Sales.Contracts.TiersDepenseFonctionnement;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.TiersDepenseFonctionnement.GetTiersDepenseFonctionnementById;

public class GetTiersDepenseFonctionnementByIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/tiers-depenses-fonctionnement/{id:int}", async (IMediator mediator, int id, CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new GetTiersDepenseFonctionnementByIdQuery(id), cancellationToken);
            if (result.IsEntityNotFound())
                return Results.NotFound();
            return Results.Ok(result.Value);
        })
        .RequireAuthorization($"Permission:{Permissions.ViewTiersDepenseFonctionnement}")
        .WithTags(EndpointTags.TiersDepenseFonctionnement);
    }
}
