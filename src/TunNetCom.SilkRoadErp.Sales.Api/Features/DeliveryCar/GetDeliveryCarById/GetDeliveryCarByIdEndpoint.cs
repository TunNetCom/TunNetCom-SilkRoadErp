using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Constants;
using TunNetCom.SilkRoadErp.Sales.Contracts.DeliveryCar;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryCar.GetDeliveryCarById;

public class GetDeliveryCarByIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/delivery-cars/{id:int}", HandleGetDeliveryCarByIdAsync)
            .WithTags(EndpointTags.DeliveryCars)
            .Produces<DeliveryCarResponse>()
            .ProducesValidationProblem();
    }

    public static async Task<Results<Ok<DeliveryCarResponse>, ValidationProblem>> HandleGetDeliveryCarByIdAsync(
        IMediator mediator,
        int id,
        CancellationToken cancellationToken)
    {
        var query = new GetDeliveryCarByIdQuery(id);
        var result = await mediator.Send(query, cancellationToken);

        if (result.IsFailed)
        {
            return result.ToValidationProblem();
        }

        return TypedResults.Ok(result.Value);
    }
}






