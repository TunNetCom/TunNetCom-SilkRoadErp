using TunNetCom.SilkRoadErp.Sales.Contracts.Soldes;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Constants;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Soldes.GetRestesALivrerParClient;

public class GetRestesALivrerParClientEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/soldes/clients-avec-problemes/restes-a-livrer", HandleGetRestesALivrerParClientAsync)
            .WithTags(EndpointTags.Soldes)
            .Produces<RestesALivrerParClientResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithDescription("Retrieves restes à livrer by product grouped by client, with reste à payer per client.")
            .WithOpenApi();
    }

    public async Task<Results<Ok<RestesALivrerParClientResponse>, NotFound>> HandleGetRestesALivrerParClientAsync(
        IMediator mediator,
        [FromQuery] int? accountingYearId,
        CancellationToken cancellationToken)
    {
        var query = new GetRestesALivrerParClientQuery(accountingYearId);
        var result = await mediator.Send(query, cancellationToken);

        if (result.IsFailed)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(result.Value);
    }
}
