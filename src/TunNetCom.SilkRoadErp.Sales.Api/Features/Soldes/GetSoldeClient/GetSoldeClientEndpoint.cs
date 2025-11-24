using TunNetCom.SilkRoadErp.Sales.Contracts.Soldes;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Soldes.GetSoldeClient;

public class GetSoldeClientEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/soldes/client/{clientId:int}", HandleGetSoldeClientAsync)
            .WithTags(EndpointTags.Soldes);
    }

    public async Task<Results<Ok<SoldeClientResponse>, NotFound>> HandleGetSoldeClientAsync(
        IMediator mediator,
        int clientId,
        int? accountingYearId,
        CancellationToken cancellationToken)
    {
        var query = new GetSoldeClientQuery(clientId, accountingYearId);
        var result = await mediator.Send(query, cancellationToken);

        if (result.IsFailed)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(result.Value);
    }
}

