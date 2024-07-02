namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Clients.GetClient;

public class GetClientsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/clients", async (IMediator mediator, HttpContext httpContext, [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10) =>
        {
            var query = new GetClientsQuery(pageIndex, pageSize);
            var result = await mediator.Send(query);

            var metadata = new
            {
                result.TotalCount,
                result.PageSize,
                result.CurrentPage,
                result.TotalPages,
                result.HasNext,
                result.HasPrevious
            };

            httpContext.Response.Headers["X-Pagination"] = JsonConvert.SerializeObject(metadata);

            return Results.Ok(result);
        });
    }
}
