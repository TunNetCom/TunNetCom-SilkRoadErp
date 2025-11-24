namespace TunNetCom.SilkRoadErp.Sales.Api.Features.PaiementClient.DeletePaiementClient;

public class DeletePaiementClientEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapDelete("/paiement-client/{id:int}", HandleDeletePaiementClientAsync)
            .WithTags(EndpointTags.PaiementClient);
    }

    public async Task<Results<NoContent, ValidationProblem>> HandleDeletePaiementClientAsync(
        IMediator mediator,
        int id,
        CancellationToken cancellationToken)
    {
        var command = new DeletePaiementClientCommand(id);
        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return result.ToValidationProblem();
        }

        return TypedResults.NoContent();
    }
}

