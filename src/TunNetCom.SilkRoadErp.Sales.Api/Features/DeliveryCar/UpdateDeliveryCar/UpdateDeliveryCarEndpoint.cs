using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Constants;
using TunNetCom.SilkRoadErp.Sales.Contracts.DeliveryCar;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryCar.UpdateDeliveryCar;

public class UpdateDeliveryCarEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPut("/delivery-cars/{id:int}", HandleUpdateDeliveryCarAsync)
            .WithTags(EndpointTags.DeliveryCars)
            .Produces<DeliveryCarResponse>()
            .ProducesValidationProblem();
    }

    public static async Task<Results<Ok<DeliveryCarResponse>, ValidationProblem>> HandleUpdateDeliveryCarAsync(
        IMediator mediator,
        int id,
        UpdateDeliveryCarRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateDeliveryCarCommand(
            Id: id,
            Matricule: request.Matricule,
            Owner: request.Owner
        );

        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return result.ToValidationProblem();
        }

        return TypedResults.Ok(result.Value);
    }
}

