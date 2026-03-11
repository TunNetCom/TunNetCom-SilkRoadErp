using TunNetCom.SilkRoadErp.Sales.Contracts.FactureAvoirFournisseur;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.FactureAvoirFournisseur.UpdateFactureAvoirFournisseur;

public class UpdateFactureAvoirFournisseurEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPut("/facture-avoir-fournisseur/{id:int}", HandleUpdateFactureAvoirFournisseurAsync)
            .WithTags(EndpointTags.FactureAvoirFournisseur);
    }

    public async Task<Results<NoContent, NotFound, ValidationProblem>> HandleUpdateFactureAvoirFournisseurAsync(
        IMediator mediator,
        int id,
        UpdateFactureAvoirFournisseurRequest updateFactureAvoirFournisseurRequest,
        CancellationToken cancellationToken)
    {
        var command = new UpdateFactureAvoirFournisseurCommand(
            id,
            updateFactureAvoirFournisseurRequest.Date,
            updateFactureAvoirFournisseurRequest.IdFournisseur,
            updateFactureAvoirFournisseurRequest.NumFactureAvoirFourSurPage,
            updateFactureAvoirFournisseurRequest.NumFactureFournisseur, // Map NumFactureFournisseur to FactureFournisseurId
            updateFactureAvoirFournisseurRequest.AvoirFournisseurIds);

        var result = await mediator.Send(command, cancellationToken);

        if (result.IsEntityNotFound())
        {
            return TypedResults.NotFound();
        }

        if (result.IsFailed)
        {
            return result.ToValidationProblem();
        }

        return TypedResults.NoContent();
    }
}

