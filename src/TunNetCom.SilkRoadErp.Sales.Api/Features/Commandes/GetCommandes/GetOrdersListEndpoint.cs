using TunNetCom.SilkRoadErp.Sales.Contracts.Commande;
using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Commandes.GetCommandes;

public class GetOrdersListEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/api/orders", async (
                [FromServices] IMediator mediator,
                CancellationToken cancellationToken) =>
        {
            var query = new GetOrdersListQuery();
            var result = await mediator.Send(query, cancellationToken);

            if (result.IsFailed)
            {
                return Results.BadRequest(new { Message = "An error occurred while retrieving the orders list.", Errors = result.Errors });
            }

            return Results.Ok(result.Value);
        })
            .Produces<List<OrderSummaryResponse>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .WithName("GetOrdersList")
            .WithTags(EndpointTags.Orders)
            .WithOpenApi(operation => new(operation)
            {
                Summary = "Retrieves a list of orders with summary information.",
                Description = "Returns a list of orders including order number, supplier ID, date, and totals (excluding VAT, VAT, and net to pay)."
            });
    }
}