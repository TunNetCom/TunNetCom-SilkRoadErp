using TunNetCom.SilkRoadErp.Sales.Contracts.Avoirs;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Avoirs.GetFullAvoir;

public class GetFullAvoirEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/avoirs/{num:int}/full", HandleGetFullAvoirAsync)
            .WithTags(EndpointTags.Avoirs);
    }

    public async Task<Results<Ok<FullAvoirResponse>, NotFound>> HandleGetFullAvoirAsync(
        IMediator mediator,
        int num,
        CancellationToken cancellationToken)
    {
        var query = new GetFullAvoirQuery(num);
        var result = await mediator.Send(query, cancellationToken);

        if (result.IsFailed)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(result.Value);
    }
}

