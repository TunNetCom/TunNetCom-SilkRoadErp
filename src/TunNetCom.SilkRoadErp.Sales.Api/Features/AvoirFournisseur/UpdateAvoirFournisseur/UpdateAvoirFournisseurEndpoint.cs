using TunNetCom.SilkRoadErp.Sales.Contracts.AvoirFournisseur;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.AvoirFournisseur.UpdateAvoirFournisseur;

public class UpdateAvoirFournisseurEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPut("/avoir-fournisseur/{num:int}", HandleUpdateAvoirFournisseurAsync)
            .WithTags(EndpointTags.AvoirFournisseur);
    }

    public async Task<Results<NoContent, NotFound, ValidationProblem>> HandleUpdateAvoirFournisseurAsync(
        IMediator mediator,
        int num,
        UpdateAvoirFournisseurRequest updateAvoirFournisseurRequest,
        CancellationToken cancellationToken)
    {
        var command = new UpdateAvoirFournisseurCommand(
            num,
            updateAvoirFournisseurRequest.Date,
            updateAvoirFournisseurRequest.FournisseurId,
            updateAvoirFournisseurRequest.NumFactureAvoirFournisseur,
            updateAvoirFournisseurRequest.Lines);

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

