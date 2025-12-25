using TunNetCom.SilkRoadErp.Sales.Contracts.AvoirFournisseur;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.AvoirFournisseur.CreateAvoirFournisseur;

public class CreateAvoirFournisseurEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/avoir-fournisseur", HandleCreateAvoirFournisseurAsync)
            .WithTags(EndpointTags.AvoirFournisseur);
    }

    public async Task<Results<Created<CreateAvoirFournisseurRequest>, ValidationProblem>> HandleCreateAvoirFournisseurAsync(
        IMediator mediator,
        CreateAvoirFournisseurRequest createAvoirFournisseurRequest,
        CancellationToken cancellationToken)
    {
        var command = new CreateAvoirFournisseurCommand(
            createAvoirFournisseurRequest.Date,
            createAvoirFournisseurRequest.FournisseurId,
            createAvoirFournisseurRequest.NumFactureAvoirFournisseur,
            createAvoirFournisseurRequest.NumAvoirChezFournisseur,
            createAvoirFournisseurRequest.Lines);

        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return result.ToValidationProblem();
        }

        return TypedResults.Created($"/avoir-fournisseur/{result.Value}", createAvoirFournisseurRequest);
    }
}

