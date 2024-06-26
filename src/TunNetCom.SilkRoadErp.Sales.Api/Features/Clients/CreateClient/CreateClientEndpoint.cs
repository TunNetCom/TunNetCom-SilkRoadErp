namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Clients.CreateClient;

public class CreateClientEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/clients", async (IMediator mediator, CreateClientRequest request) =>
        {
            var command = new CreateClientCommand(request);
            var result = await mediator.Send(command);
            return Results.Created($"/clients/{result.Id}", result);
        });
    }
}
