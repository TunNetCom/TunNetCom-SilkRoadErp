
using TunNetCom.SilkRoadErp.Sales.Contracts.Products;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Products.GetProductByRef
{
    public class GetProductByRefEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/Products/{refe}", async Task<Results<Ok<ProductResponse>, NotFound>> (IMediator mediator, string refe) =>
            {
                var query = new GetProductByRefQuery(refe);

                if (query is null)
                {
                    return TypedResults.NotFound();
                }

                var result = await mediator.Send(query);
                return TypedResults.Ok(result.Value);


            });
        }
    }
}
