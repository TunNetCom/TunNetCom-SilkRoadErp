using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Constants;
using TunNetCom.SilkRoadErp.Sales.Contracts.Tags;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Tags.UpdateTag;

public class UpdateTagEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPut("/tags/{id:int}", HandleUpdateTagAsync)
            .WithTags(EndpointTags.Tags)
            .Produces<TagResponse>()
            .ProducesValidationProblem();
    }

    public static async Task<Results<Ok<TagResponse>, ValidationProblem>> HandleUpdateTagAsync(
        IMediator mediator,
        int id,
        UpdateTagRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateTagCommand(
            Id: id,
            Name: request.Name,
            Color: request.Color,
            Description: request.Description
        );

        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return result.ToValidationProblem();
        }

        return TypedResults.Ok(result.Value);
    }
}

