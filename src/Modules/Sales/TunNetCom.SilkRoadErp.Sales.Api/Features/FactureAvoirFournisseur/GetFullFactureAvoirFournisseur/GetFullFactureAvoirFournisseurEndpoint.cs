using TunNetCom.SilkRoadErp.Sales.Contracts.FactureAvoirFournisseur;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.FactureAvoirFournisseur.GetFullFactureAvoirFournisseur;

public class GetFullFactureAvoirFournisseurEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/facture-avoir-fournisseur/{id:int}/full", HandleGetFullFactureAvoirFournisseurAsync)
            .WithTags(EndpointTags.FactureAvoirFournisseur);
    }

    public async Task<Results<Ok<FullFactureAvoirFournisseurResponse>, NotFound>> HandleGetFullFactureAvoirFournisseurAsync(
        IMediator mediator,
        int id,
        CancellationToken cancellationToken)
    {
        var query = new GetFullFactureAvoirFournisseurQuery(id);
        var result = await mediator.Send(query, cancellationToken);

        if (result.IsFailed)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(result.Value);
    }
}

