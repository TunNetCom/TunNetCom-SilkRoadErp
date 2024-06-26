namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Clients.UpdateClient;

public class UpdateClientEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/clients/{id:int}", async (IMediator mediator, int id, UpdateClientRequest request) =>
        {
            var command = new UpdateClientCommand(id, request);
            var result = await mediator.Send(command);
            return Results.Ok(result);
        });
    }
}
