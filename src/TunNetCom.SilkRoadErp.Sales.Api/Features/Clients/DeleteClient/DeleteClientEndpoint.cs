namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Clients.DeleteClient;

public class DeleteClientEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/clients/{id:int}", async Task<Results<NoContent, NotFound>> (IMediator mediator, int id) =>
        {
            var deleteClientCommand = new DeleteClientCommand(id);
            var deleteResult = await mediator.Send(deleteClientCommand);

            //TODO conditions based on the result : business validations and not found case.
            if(deleteResult.IsFailed)
            {
                return TypedResults.NotFound();
            }

            return TypedResults.NoContent();
        });
    }
}
