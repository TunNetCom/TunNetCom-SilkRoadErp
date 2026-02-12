using Carter;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Constants;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.ResultExtensions;
using TunNetCom.SilkRoadErp.Sales.Contracts.TiersDepenseFonctionnement;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.TiersDepenseFonctionnement.UpdateTiersDepenseFonctionnement;

public class UpdateTiersDepenseFonctionnementEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPut("/tiers-depenses-fonctionnement/{id:int}", async (IMediator mediator, int id, UpdateTiersDepenseFonctionnementRequest request, CancellationToken cancellationToken) =>
        {
            var command = new UpdateTiersDepenseFonctionnementCommand(
                id,
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
                return result.IsEntityNotFound() ? Results.NotFound() : Results.BadRequest(result.Errors);
            return Results.NoContent();
        })
        .RequireAuthorization($"Permission:{Permissions.UpdateTiersDepenseFonctionnement}")
        .WithTags(EndpointTags.TiersDepenseFonctionnement);
    }
}
