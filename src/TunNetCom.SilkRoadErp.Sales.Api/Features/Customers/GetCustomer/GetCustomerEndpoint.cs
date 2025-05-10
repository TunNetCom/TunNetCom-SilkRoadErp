namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Customers.GetCustomer;

public class GetCustomersEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/customers", async (
                [FromQuery] int pageNumber,
                [FromQuery] int pageSize,
                [FromQuery] string? searchKeyword,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                if (pageNumber < 1 || pageSize < 1)
                {
                    return Results.Problem(
                        statusCode: StatusCodes.Status400BadRequest,
                        detail: "PageNumber and PageSize must be greater than zero.");
                }

                var query = new GetCustomerQuery (pageNumber, pageSize, searchKeyword);

                var pagedCustomers = await mediator.Send(query, cancellationToken);
                return Results.Ok(pagedCustomers);
            })
            .Produces<PagedList<CustomerResponse>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithDescription("Retrieves a paginated list of customers with optional search filtering.")
            .WithOpenApi();
    }
}
