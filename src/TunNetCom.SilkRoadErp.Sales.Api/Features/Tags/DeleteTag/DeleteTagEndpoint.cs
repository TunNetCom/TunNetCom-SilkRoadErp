using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Constants;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Tags.DeleteTag;

public class DeleteTagEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapDelete("/tags/{id:int}", HandleDeleteTagAsync)
            .WithTags(EndpointTags.Tags)
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem();
    }

    public static async Task<Results<NoContent, ValidationProblem>> HandleDeleteTagAsync(
        IMediator mediator,
        int id,
        CancellationToken cancellationToken)
    {
        var command = new DeleteTagCommand(id);
        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return result.ToValidationProblem();
        }

        return TypedResults.NoContent();
    }
}

