using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Constants;
using TunNetCom.SilkRoadErp.Sales.Contracts.DeliveryCar;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryCar.GetAllDeliveryCars;

public class GetAllDeliveryCarsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/delivery-cars", HandleGetAllDeliveryCarsAsync)
            .WithTags(EndpointTags.DeliveryCars)
            .Produces<List<DeliveryCarResponse>>();
    }

    public static async Task<Ok<List<DeliveryCarResponse>>> HandleGetAllDeliveryCarsAsync(
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query = new GetAllDeliveryCarsQuery();
        var result = await mediator.Send(query, cancellationToken);
        return TypedResults.Ok(result);
    }
}





