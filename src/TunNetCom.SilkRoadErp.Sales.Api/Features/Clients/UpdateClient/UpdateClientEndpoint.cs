namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Clients.UpdateClient;

public class UpdateClientEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/clients/{id:int}", async (IMediator mediator, int id, UpdateClientRequest request) =>
        {
            try
            {
                var command = new UpdateClientCommand(id, request);
                var result = await mediator.Send(command);
                return Results.Ok(result);
            }
            catch (ValidationException e)
            {
                var errors = e.Errors
                       .GroupBy(error => error.PropertyName)
                       .ToDictionary(
                           group => group.Key,
                           group => group.Select(error => error.ErrorMessage).ToArray()
                       );

                return Results.ValidationProblem(errors);
            }
        });
    }
}
