using Carter;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Constants;
using TunNetCom.SilkRoadErp.Sales.Contracts.Products;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Products.GetProductsStock;

public class GetProductsStockEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/products/stock", async (IMediator mediator, List<string> refProduits, CancellationToken cancellationToken) =>
        {
            var query = new GetProductsStockQuery(refProduits);
            var result = await mediator.Send(query, cancellationToken);

            if (result.IsFailed)
            {
                return Results.NotFound();
            }

            return Results.Ok(result.Value);
        })
        .WithTags(EndpointTags.Products)
        .Produces<List<ProductStockResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}

