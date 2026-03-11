using TunNetCom.SilkRoadErp.Sales.Contracts.RetourMarchandiseFournisseur;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.RetourMarchandiseFournisseur.GetRetourMarchandiseFournisseur;

public class GetRetourMarchandiseFournisseurEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/retour-marchandise-fournisseur/{num:int}",
            async (int num,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var query = new GetRetourMarchandiseFournisseurQuery(num);
            var result = await mediator.Send(query, cancellationToken);
            
            if (result.IsFailed)
            {
                return Results.NotFound();
            }
            
            return Results.Ok(result.Value);
        })
        .WithTags(EndpointTags.RetourMarchandiseFournisseur)
        .WithName("GetRetourMarchandiseFournisseur")
        .WithSummary("Get a supplier return by number")
        .Produces<RetourMarchandiseFournisseurResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);
    }
}

