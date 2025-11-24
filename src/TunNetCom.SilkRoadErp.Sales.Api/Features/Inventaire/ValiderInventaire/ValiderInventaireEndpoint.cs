using FluentResults;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Inventaire.ValiderInventaire;

public class ValiderInventaireEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPost(
            "/inventaires/{id:int}/valider",
            async (IMediator mediator, int id, CancellationToken cancellationToken) =>
            {
                var command = new ValiderInventaireCommand(Id: id);
                var result = await mediator.Send(command, cancellationToken);
                
                if (result.IsFailed)
                {
                    return result.ToValidationProblem();
                }
                
                return Results.NoContent();
            })
            .WithTags("Inventaire");
    }
}

