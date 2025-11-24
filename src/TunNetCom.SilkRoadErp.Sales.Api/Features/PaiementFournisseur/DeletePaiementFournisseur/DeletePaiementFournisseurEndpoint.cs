namespace TunNetCom.SilkRoadErp.Sales.Api.Features.PaiementFournisseur.DeletePaiementFournisseur;

public class DeletePaiementFournisseurEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapDelete("/paiement-fournisseur/{id:int}", HandleDeletePaiementFournisseurAsync)
            .WithTags(EndpointTags.PaiementFournisseur);
    }

    public async Task<Results<NoContent, ValidationProblem>> HandleDeletePaiementFournisseurAsync(
        IMediator mediator,
        int id,
        CancellationToken cancellationToken)
    {
        var command = new DeletePaiementFournisseurCommand(id);
        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return result.ToValidationProblem();
        }

        return TypedResults.NoContent();
    }
}

