using TunNetCom.SilkRoadErp.Sales.Contracts.Commande;
namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Commandes.GetCommandes;
public class OrderEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/api/orders/{id}", async (
                [FromRoute] int id,
                [FromServices] IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var query = new GetFullOrderQuery(id);
                var result = await mediator.Send(query, cancellationToken);

                if (result.IsFailed)
                {
                    return result.Errors.Any(e => e is EntityNotFound)
                        ? Results.NotFound(new { Message = $"Order with ID {id} not found." })
                        : Results.BadRequest(new { Message = "An error occurred while retrieving the order.", Errors = result.Errors });
                }

                return Results.Ok(result.Value);
            })
            .Produces<FullOrderResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest)
            .WithName("GetFullOrder")
            .WithTags(EndpointTags.Orders)
            .WithOpenApi(operation => new(operation)
            {
                Summary = "Retrieves a full order by its ID, including supplier details and order lines.",
                Description = "Returns the full order details with supplier information and order lines, or an error if the order is not found."
            });
    }
}