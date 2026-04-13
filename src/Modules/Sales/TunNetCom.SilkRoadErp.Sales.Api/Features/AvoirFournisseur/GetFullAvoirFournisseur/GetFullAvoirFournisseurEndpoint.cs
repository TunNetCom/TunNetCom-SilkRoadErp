using TunNetCom.SilkRoadErp.Sales.Contracts.AvoirFournisseur;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.AvoirFournisseur.GetFullAvoirFournisseur;

public class GetFullAvoirFournisseurEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/avoir-fournisseur/{id:int}/full", HandleGetFullAvoirFournisseurAsync)
            .WithTags(EndpointTags.AvoirFournisseur);
    }

    public async Task<Results<Ok<FullAvoirFournisseurResponse>, NotFound>> HandleGetFullAvoirFournisseurAsync(
        IMediator mediator,
        int id,
        CancellationToken cancellationToken)
    {
        var query = new GetFullAvoirFournisseurQuery(id);
        var result = await mediator.Send(query, cancellationToken);

        if (result.IsFailed)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(result.Value);
    }
}

