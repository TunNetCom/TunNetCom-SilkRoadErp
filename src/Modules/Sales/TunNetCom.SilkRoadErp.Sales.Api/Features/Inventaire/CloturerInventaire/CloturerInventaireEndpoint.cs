using FluentResults;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Inventaire.CloturerInventaire;

public class CloturerInventaireEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPost(
            "/inventaires/{id:int}/cloturer",
            async (IMediator mediator, int id, CancellationToken cancellationToken) =>
            {
                var command = new CloturerInventaireCommand(Id: id);
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

