namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Providers.DeleteProvider;

public class DeleteProviderEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/providers/{id:int}", async Task<Results<NoContent, NotFound>> (IMediator mediator, int id, CancellationToken cancellationToken) =>
        {
            var deleteProviderCommand = new DeleteProviderCommand(id);
            var deleteResult = await mediator.Send(deleteProviderCommand, cancellationToken);

            if (deleteResult.IsFailed)
            {
                return TypedResults.NotFound();
            }

            return TypedResults.NoContent();
        });
    }
}
