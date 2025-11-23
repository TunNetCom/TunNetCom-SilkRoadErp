using TunNetCom.SilkRoadErp.Sales.Contracts.FactureAvoirFournisseur;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.FactureAvoirFournisseur.CreateFactureAvoirFournisseur;

public class CreateFactureAvoirFournisseurEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/facture-avoir-fournisseur", HandleCreateFactureAvoirFournisseurAsync)
            .WithTags(EndpointTags.FactureAvoirFournisseur);
    }

    public async Task<Results<Created<CreateFactureAvoirFournisseurRequest>, ValidationProblem>> HandleCreateFactureAvoirFournisseurAsync(
        IMediator mediator,
        CreateFactureAvoirFournisseurRequest createFactureAvoirFournisseurRequest,
        CancellationToken cancellationToken)
    {
        var command = new CreateFactureAvoirFournisseurCommand(
            createFactureAvoirFournisseurRequest.Date,
            createFactureAvoirFournisseurRequest.IdFournisseur,
            createFactureAvoirFournisseurRequest.NumFactureFournisseur,
            createFactureAvoirFournisseurRequest.AvoirFournisseurIds);

        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return result.ToValidationProblem();
        }

        return TypedResults.Created($"/facture-avoir-fournisseur/{result.Value}", createFactureAvoirFournisseurRequest);
    }
}

