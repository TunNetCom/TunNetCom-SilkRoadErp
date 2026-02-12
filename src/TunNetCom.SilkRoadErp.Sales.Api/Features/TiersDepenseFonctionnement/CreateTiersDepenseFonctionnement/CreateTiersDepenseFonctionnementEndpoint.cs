using Carter;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Constants;
using TunNetCom.SilkRoadErp.Sales.Contracts.TiersDepenseFonctionnement;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.TiersDepenseFonctionnement.CreateTiersDepenseFonctionnement;

public class CreateTiersDepenseFonctionnementEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/tiers-depenses-fonctionnement", async (IMediator mediator, CreateTiersDepenseFonctionnementRequest request, CancellationToken cancellationToken) =>
        {
            var command = new CreateTiersDepenseFonctionnementCommand(
                request.Nom,
                request.Tel,
                request.Adresse,
                request.Matricule,
                request.Code,
                request.CodeCat,
                request.EtbSec,
                request.Mail);
            var result = await mediator.Send(command, cancellationToken);
            if (result.IsFailed)
                return Results.BadRequest(result.Errors);
            return Results.Created($"/tiers-depenses-fonctionnement/{result.Value}", new { id = result.Value });
        })
        .RequireAuthorization($"Permission:{Permissions.CreateTiersDepenseFonctionnement}")
        .WithTags(EndpointTags.TiersDepenseFonctionnement);
    }
}
