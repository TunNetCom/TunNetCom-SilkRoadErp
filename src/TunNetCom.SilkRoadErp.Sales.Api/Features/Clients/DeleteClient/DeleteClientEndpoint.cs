namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Clients.DeleteClient;

public class DeleteClientEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/clients/{id:int}", async (IMediator mediator, int id) =>
        {
            var command = new DeleteClientCommand(id);
            await mediator.Send(command);
            return Results.NoContent();
        });
    }
}
