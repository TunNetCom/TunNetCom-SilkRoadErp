namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Customers.GetCustomer;

public class GetCustomersEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/customers", HandleGetCustomersAsync)
            .Produces<PagedList<CustomerResponse>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithDescription("Retrieves a paginated list of customers.");
    }

    public static async Task<Results<Ok<PagedList<CustomerResponse>>, ValidationProblem>> HandleGetCustomersAsync(
        [AsParameters] QueryStringParameters paginationQueryParams,
        IMediator mediator,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var query = new GetCustomerQuery(
            paginationQueryParams.PageNumber,
            paginationQueryParams.PageSize,
            paginationQueryParams.SearchKeyword);

        var pagedCustomers = await mediator.Send(query, cancellationToken);

        var metadata = new
        {
            pagedCustomers.TotalCount,
            pagedCustomers.PageSize,
            pagedCustomers.CurrentPage,
            pagedCustomers.TotalPages,
            pagedCustomers.HasNext,
            pagedCustomers.HasPrevious
        };

        httpContext.Response.Headers["X-Pagination"] = JsonConvert.SerializeObject(metadata);

        return TypedResults.Ok(pagedCustomers);
    }
}
