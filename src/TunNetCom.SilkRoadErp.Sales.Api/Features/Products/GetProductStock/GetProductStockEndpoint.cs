using Carter;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Constants;
using TunNetCom.SilkRoadErp.Sales.Contracts.Products;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Products.GetProductStock;

public class GetProductStockEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/products/{refProduit}/stock", async (IMediator mediator, string refProduit, CancellationToken cancellationToken) =>
        {
            var query = new GetProductStockQuery(refProduit);
            var result = await mediator.Send(query, cancellationToken);

            if (result.IsFailed)
            {
                return Results.NotFound();
            }

            return Results.Ok(result.Value);
        })
        .RequireAuthorization($"Permission:{Permissions.ViewProductStock}")
        .WithTags(EndpointTags.Products)
        .Produces<ProductStockResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}

