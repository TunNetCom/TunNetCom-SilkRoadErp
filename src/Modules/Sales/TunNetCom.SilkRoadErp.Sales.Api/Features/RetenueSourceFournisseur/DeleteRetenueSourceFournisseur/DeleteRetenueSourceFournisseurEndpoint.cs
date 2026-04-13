namespace TunNetCom.SilkRoadErp.Sales.Api.Features.RetenueSourceFournisseur.DeleteRetenueSourceFournisseur;

public class DeleteRetenueSourceFournisseurEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapDelete("/retenue-source-fournisseur/{numFactureFournisseur:int}", HandleDeleteRetenueSourceFournisseurAsync)
            .WithTags("RetenueSourceFournisseur")
            .RequireAuthorization();
    }

    public async Task<Results<NoContent, ValidationProblem>> HandleDeleteRetenueSourceFournisseurAsync(
        IMediator mediator,
        int numFactureFournisseur,
        CancellationToken cancellationToken)
    {
        var command = new DeleteRetenueSourceFournisseurCommand(numFactureFournisseur);
        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return result.ToValidationProblem();
        }

        return TypedResults.NoContent();
    }
}


