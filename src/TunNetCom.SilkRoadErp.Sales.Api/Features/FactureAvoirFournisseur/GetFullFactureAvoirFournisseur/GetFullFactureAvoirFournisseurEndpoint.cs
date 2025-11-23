using TunNetCom.SilkRoadErp.Sales.Contracts.FactureAvoirFournisseur;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.FactureAvoirFournisseur.GetFullFactureAvoirFournisseur;

public class GetFullFactureAvoirFournisseurEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/facture-avoir-fournisseur/{num:int}/full", HandleGetFullFactureAvoirFournisseurAsync)
            .WithTags(EndpointTags.FactureAvoirFournisseur);
    }

    public async Task<Results<Ok<FullFactureAvoirFournisseurResponse>, NotFound>> HandleGetFullFactureAvoirFournisseurAsync(
        IMediator mediator,
        int num,
        CancellationToken cancellationToken)
    {
        var query = new GetFullFactureAvoirFournisseurQuery(num);
        var result = await mediator.Send(query, cancellationToken);

        if (result.IsFailed)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(result.Value);
    }
}

