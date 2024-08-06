namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Products.GetProductByRef;
public class GetProductByRefEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/products/{refe}", async Task<Results<Ok<ProductResponse>, NotFound>> (
            IMediator mediator, 
            string refe,
            CancellationToken cancellationToken) =>
        {
            var query = new GetProductByRefQuery(refe);
            var result = await mediator.Send(query, cancellationToken);

            if (result.IsFailed)
            {
                return TypedResults.NotFound();
            }
            return TypedResults.Ok(result.Value);
        });
    }
}