namespace TunNetCom.SilkRoadErp.Sales.Api.Features.FactureDepense.DeleteFactureDepense;

public class DeleteFactureDepenseEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapDelete("/factures-depenses/{id:int}", HandleDeleteFactureDepenseAsync)
            .WithTags(EndpointTags.FactureDepense);
    }

    public async Task<Results<NoContent, ValidationProblem>> HandleDeleteFactureDepenseAsync(
        IMediator mediator,
        int id,
        CancellationToken cancellationToken)
    {
        var command = new DeleteFactureDepenseCommand(id);
        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailed)
            return result.ToValidationProblem();

        return TypedResults.NoContent();
    }
}
