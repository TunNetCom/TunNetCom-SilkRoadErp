using TunNetCom.SilkRoadErp.Sales.Contracts.FactureAvoirFournisseur;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.FactureAvoirFournisseur.GetFactureAvoirFournisseur;

public class GetFactureAvoirFournisseurEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/facture-avoir-fournisseur/{id:int}", HandleGetFactureAvoirFournisseurAsync)
            .WithTags(EndpointTags.FactureAvoirFournisseur);
    }

    public async Task<Results<Ok<FactureAvoirFournisseurResponse>, NotFound>> HandleGetFactureAvoirFournisseurAsync(
        IMediator mediator,
        int id,
        CancellationToken cancellationToken)
    {
        var query = new GetFactureAvoirFournisseurQuery(id);
        var result = await mediator.Send(query, cancellationToken);

        if (result.IsFailed)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(result.Value);
    }
}

