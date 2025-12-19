using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Constants;
using TunNetCom.SilkRoadErp.Sales.Contracts.DeliveryCar;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryCar.CreateDeliveryCar;

public class CreateDeliveryCarEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/delivery-cars", HandleCreateDeliveryCarAsync)
            .WithTags(EndpointTags.DeliveryCars)
            .Produces<DeliveryCarResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem();
    }

    public static async Task<Results<Created<DeliveryCarResponse>, ValidationProblem>> HandleCreateDeliveryCarAsync(
        IMediator mediator,
        CreateDeliveryCarRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateDeliveryCarCommand(
            Matricule: request.Matricule,
            Owner: request.Owner
        );

        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return result.ToValidationProblem();
        }

        return TypedResults.Created($"/delivery-cars/{result.Value.Id}", result.Value);
    }
}






