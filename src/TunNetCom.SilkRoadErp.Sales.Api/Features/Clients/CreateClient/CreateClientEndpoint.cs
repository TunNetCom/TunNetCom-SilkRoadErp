namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Clients.CreateClient
{
    public class CreateClientEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/clients", async (IMediator mediator, CreateClientRequest request) =>
            {
                try
                {
                    var command = new CreateClientCommand(request);
                    var result = await mediator.Send(command);
                    return Results.Created($"/clients/{result.Id}", result);
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
}
