namespace TunNetCom.SilkRoadErp.Sales.Api.Features.PaiementTiersDepense.DeletePaiementTiersDepense;

public class DeletePaiementTiersDepenseEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapDelete("/paiements-tiers-depenses/{id:int}", HandleDeletePaiementTiersDepenseAsync)
            .WithTags(EndpointTags.PaiementTiersDepense);
    }

    public async Task<Results<NoContent, ValidationProblem>> HandleDeletePaiementTiersDepenseAsync(
        IMediator mediator,
        int id,
        CancellationToken cancellationToken)
    {
        var command = new DeletePaiementTiersDepenseCommand(id);
        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailed)
            return result.ToValidationProblem();

        return TypedResults.NoContent();
    }
}
