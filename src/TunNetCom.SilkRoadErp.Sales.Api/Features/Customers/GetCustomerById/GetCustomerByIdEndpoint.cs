using TunNetCom.SilkRoadErp.Sales.Contracts.Customers;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Customers.GetCustomerById;

public class GetCustomerByIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/customers/{id:int}", async Task<Results<Ok<CustomerResponse>, NotFound>> (
            IMediator mediator,
            int id,
            CancellationToken cancellationToken) =>
        {
            var query = new GetCustomerByIdQuery(id);

            var result = await mediator.Send(query, cancellationToken);

            if (result.IsFailed)
            {
                return TypedResults.NotFound();
            }

            return TypedResults.Ok(result.Value);
        });
    }
}
