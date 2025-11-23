using TunNetCom.SilkRoadErp.Sales.Contracts.AvoirFournisseur;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.AvoirFournisseur.GetAvoirFournisseur;

public class GetAvoirFournisseurEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/avoir-fournisseur/{num:int}", HandleGetAvoirFournisseurAsync)
            .WithTags(EndpointTags.AvoirFournisseur);
    }

    public async Task<Results<Ok<AvoirFournisseurResponse>, NotFound>> HandleGetAvoirFournisseurAsync(
        IMediator mediator,
        int num,
        CancellationToken cancellationToken)
    {
        var query = new GetAvoirFournisseurQuery(num);
        var result = await mediator.Send(query, cancellationToken);

        if (result.IsFailed)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(result.Value);
    }
}

