namespace TunNetCom.SilkRoadErp.Sales.Api.Features.priceQuote.GetPriceQuote;

public class GetPriceQuoteEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/quotations", async (
             [AsParameters] QueryStringParameters paginationQueryParams,
             IMediator mediator,
             HttpContext httpContext,
             CancellationToken cancellationToken) =>
        {
            var query = new GetPriceQuoteQuery(
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

