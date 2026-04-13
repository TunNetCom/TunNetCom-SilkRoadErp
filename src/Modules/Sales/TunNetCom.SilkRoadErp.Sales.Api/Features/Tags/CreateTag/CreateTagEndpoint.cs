using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Constants;
using TunNetCom.SilkRoadErp.Sales.Contracts.Tags;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Tags.CreateTag;

public class CreateTagEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/tags", HandleCreateTagAsync)
            .WithTags(EndpointTags.Tags)
            .Produces<TagResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem();
    }

    public static async Task<Results<Created<TagResponse>, ValidationProblem>> HandleCreateTagAsync(
        IMediator mediator,
        CreateTagRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateTagCommand(
            Name: request.Name,
            Color: request.Color,
            Description: request.Description
        );

        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return result.ToValidationProblem();
        }

        return TypedResults.Created($"/tags/{result.Value.Id}", result.Value);
    }
}

