using FluentResults;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Inventaire.GetInventaireById;

public class GetInventaireByIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet(
            "/inventaires/{id:int}",
            async (IMediator mediator, int id, CancellationToken cancellationToken) =>
            {
                var query = new GetInventaireByIdQuery(Id: id);
                var result = await mediator.Send(query, cancellationToken);
                
                if (result.IsFailed)
                {
                    return Results.NotFound();
                }
                
                return Results.Ok(result.Value);
            })
            .WithTags("Inventaire");
    }
}

