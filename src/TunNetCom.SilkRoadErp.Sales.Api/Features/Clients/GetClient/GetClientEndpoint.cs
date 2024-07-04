namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Clients.GetClient;

public class GetClientsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/clients", async (
            [AsParameters] QueryStringParameters paginationQueryParams,
            IMediator mediator,
            HttpContext httpContext) =>
        {
            var query = new GetClientsQuery(
                paginationQueryParams.PageNumber,
                paginationQueryParams.PageSize,
                paginationQueryParams.SearchKeyword);

            var pagedClients = await mediator.Send(query);

            var metadata = new
            {
                pagedClients.TotalCount,
                pagedClients.PageSize,
                pagedClients.CurrentPage,
                pagedClients.TotalPages,
                pagedClients.HasNext,
                pagedClients.HasPrevious
            };

            httpContext.Response.Headers["X-Pagination"] = JsonConvert.SerializeObject(metadata);

            return Results.Ok(pagedClients);
        });
    }
}
