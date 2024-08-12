namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Products.DeleteProduct;

public class DeleteProductEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/products/{refe}", async Task<Results<NoContent, NotFound>> (IMediator mediator, string refe, CancellationToken cancellationToken) =>
        {
            var deleteProductCommand = new DeleteProductCommand(refe);
            var deleteResult = await mediator.Send(deleteProductCommand,cancellationToken);
            if (deleteResult.IsFailed)
            {
                return TypedResults.NotFound();
            }

            return TypedResults.NoContent();
        });
    }
}
