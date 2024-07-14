namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Customers.GetCustomer;

public class GetCustomersEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/customers", async (
            [AsParameters] QueryStringParameters paginationQueryParams,
            IMediator mediator,
            HttpContext httpContext,
            CancellationToken cancellationToken) =>
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

            return Results.Ok(pagedCustomers);
        });
    }
}
