using FluentResults;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Inventaire.DeleteInventaire;

public class DeleteInventaireEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapDelete(
            "/inventaires/{id:int}",
            async (IMediator mediator, int id, CancellationToken cancellationToken) =>
            {
                var command = new DeleteInventaireCommand(Id: id);
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

