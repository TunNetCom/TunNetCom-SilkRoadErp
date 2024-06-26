namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Clients.GetClientById;

public class GetClientByIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/clients/{id:int}", async (IMediator mediator, int id) =>
        {
            var query = new GetClientByIdQuery(id);
            var result = await mediator.Send(query);
            return Results.Ok(result);
        });
    }
}
