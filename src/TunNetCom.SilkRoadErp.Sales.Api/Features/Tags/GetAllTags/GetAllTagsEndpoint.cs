using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Constants;
using TunNetCom.SilkRoadErp.Sales.Contracts.Tags;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Tags.GetAllTags;

public class GetAllTagsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/tags", HandleGetAllTagsAsync)
            .WithTags(EndpointTags.Tags)
            .Produces<List<TagResponse>>();
    }

    public static async Task<Ok<List<TagResponse>>> HandleGetAllTagsAsync(
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query = new GetAllTagsQuery();
        var result = await mediator.Send(query, cancellationToken);
        return TypedResults.Ok(result);
    }
}

