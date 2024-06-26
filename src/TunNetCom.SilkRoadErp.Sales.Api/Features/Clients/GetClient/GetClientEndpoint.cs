namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Clients.GetClient;

public class GetClientsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/clients", async (IMediator mediator, [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10) =>
        {
            var query = new GetClientsQuery(pageIndex, pageSize);
            var result = await mediator.Send(query);
            return Results.Ok(result);
        });
    }
}
