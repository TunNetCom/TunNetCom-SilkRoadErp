using FluentResults;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Inventaire.GetDernierPrixAchat;

public class GetDernierPrixAchatEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet(
            "/inventaires/dernier-prix-achat/{refProduit}",
            async (IMediator mediator, string refProduit, CancellationToken cancellationToken) =>
            {
                var query = new GetDernierPrixAchatQuery(RefProduit: refProduit);
                var result = await mediator.Send(query, cancellationToken);
                
                if (result.IsFailed)
                {
                    return Results.NotFound();
                }
                
                return Results.Ok(new { dernierPrixAchat = result.Value });
            })
            .WithTags("Inventaire");
    }
}

