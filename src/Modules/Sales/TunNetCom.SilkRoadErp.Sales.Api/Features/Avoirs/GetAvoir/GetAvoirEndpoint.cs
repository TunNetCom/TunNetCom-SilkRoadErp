using TunNetCom.SilkRoadErp.Sales.Contracts.Avoirs;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Avoirs.GetAvoir;

public class GetAvoirEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/avoirs/{num:int}", HandleGetAvoirAsync)
            .WithTags(EndpointTags.Avoirs);
    }

    public async Task<Results<Ok<AvoirResponse>, NotFound>> HandleGetAvoirAsync(
        IMediator mediator,
        int num,
        CancellationToken cancellationToken)
    {
        var query = new GetAvoirQuery(num);
        var result = await mediator.Send(query, cancellationToken);

        if (result.IsFailed)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(result.Value);
    }
}

