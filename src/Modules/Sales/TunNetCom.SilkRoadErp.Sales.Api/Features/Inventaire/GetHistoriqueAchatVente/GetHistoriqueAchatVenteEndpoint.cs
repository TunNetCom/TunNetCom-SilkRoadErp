using FluentResults;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Inventaire.GetHistoriqueAchatVente;

public class GetHistoriqueAchatVenteEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet(
            "/inventaires/historique-achat-vente/{productId:int}",
            async (IMediator mediator, int productId, CancellationToken cancellationToken) =>
            {
                var query = new GetHistoriqueAchatVenteQuery(ProductId: productId);
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

