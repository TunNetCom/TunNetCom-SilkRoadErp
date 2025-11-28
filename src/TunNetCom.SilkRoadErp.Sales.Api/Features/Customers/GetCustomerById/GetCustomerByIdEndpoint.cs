namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Customers.GetCustomerById;

public class GetCustomerByIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/customers/{id:int}", HandleGetCustomerByIdAsync)
            .RequireAuthorization($"Permission:{Permissions.ViewCustomers}")
            .WithTags(EndpointTags.Customers);
    }

    public static async Task<Results<Ok<CustomerResponse>, NotFound>> HandleGetCustomerByIdAsync(
        IMediator mediator, int id, CancellationToken cancellationToken)
    {
        var query = new GetCustomerByIdQuery(id);
        var result = await mediator.Send(query, cancellationToken);

        return result.IsEntityNotFound() ? TypedResults.NotFound() : TypedResults.Ok(result.Value);
    }
}
