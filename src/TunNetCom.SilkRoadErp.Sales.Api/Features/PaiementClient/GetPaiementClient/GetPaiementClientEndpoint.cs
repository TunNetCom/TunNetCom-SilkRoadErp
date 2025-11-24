using TunNetCom.SilkRoadErp.Sales.Contracts.PaiementClient;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.PaiementClient.GetPaiementClient;

public class GetPaiementClientEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/paiement-client/{id:int}", HandleGetPaiementClientAsync)
            .WithTags(EndpointTags.PaiementClient);
    }

    public async Task<Results<Ok<PaiementClientResponse>, NotFound>> HandleGetPaiementClientAsync(
        IMediator mediator,
        int id,
        CancellationToken cancellationToken)
    {
        var query = new GetPaiementClientQuery(id);
        var result = await mediator.Send(query, cancellationToken);

        if (result.IsFailed)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(result.Value);
    }
}

