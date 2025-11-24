using FluentResults;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Inventaire.GetHistoriqueAchatVente;

public class GetHistoriqueAchatVenteEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet(
            "/inventaires/historique-achat-vente/{refProduit}",
            async (IMediator mediator, string refProduit, CancellationToken cancellationToken) =>
            {
                var query = new GetHistoriqueAchatVenteQuery(RefProduit: refProduit);
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

