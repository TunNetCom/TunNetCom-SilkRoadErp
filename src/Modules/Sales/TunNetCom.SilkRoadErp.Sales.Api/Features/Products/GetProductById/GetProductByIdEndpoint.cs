namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Products.GetProductById;
public class GetProductByIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/products/{id:int}", async Task<Results<Ok<ProductResponse>, NotFound>> (
            IMediator mediator,
            int id,
            CancellationToken cancellationToken) =>
        {
            var query = new GetProductByIdQuery(id);
            var result = await mediator.Send(query, cancellationToken);

            if (result.IsEntityNotFound())
            {
                return TypedResults.NotFound();
            }
            return TypedResults.Ok(result.Value);
        })
        .RequireAuthorization($"Permission:{Permissions.ViewProducts}")
        .WithTags(EndpointTags.Products);
    }
}

