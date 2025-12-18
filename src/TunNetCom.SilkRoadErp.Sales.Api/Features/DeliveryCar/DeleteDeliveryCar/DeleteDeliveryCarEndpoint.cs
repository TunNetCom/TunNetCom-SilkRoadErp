using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Constants;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryCar.DeleteDeliveryCar;

public class DeleteDeliveryCarEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapDelete("/delivery-cars/{id:int}", HandleDeleteDeliveryCarAsync)
            .WithTags(EndpointTags.DeliveryCars)
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem();
    }

    public static async Task<Results<NoContent, ValidationProblem>> HandleDeleteDeliveryCarAsync(
        IMediator mediator,
        int id,
        CancellationToken cancellationToken)
    {
        var command = new DeleteDeliveryCarCommand(id);
        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return result.ToValidationProblem();
        }

        return TypedResults.NoContent();
    }
}




