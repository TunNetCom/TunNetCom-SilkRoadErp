using TunNetCom.SilkRoadErp.Sales.Contracts.PaiementClient;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.PaiementClient.CreatePaiementClient;

public class CreatePaiementClientEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/paiement-client", HandleCreatePaiementClientAsync)
            .WithTags(EndpointTags.PaiementClient);
    }

    public async Task<Results<Created<CreatePaiementClientRequest>, ValidationProblem>> HandleCreatePaiementClientAsync(
        IMediator mediator,
        CreatePaiementClientRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreatePaiementClientCommand(
            request.Numero,
            request.ClientId,
            request.Montant,
            request.DatePaiement,
            request.MethodePaiement,
            request.FactureIds,
            request.BonDeLivraisonIds,
            request.NumeroChequeTraite,
            request.BanqueId,
            request.DateEcheance,
            request.Commentaire,
            request.DocumentBase64);

        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return result.ToValidationProblem();
        }

        return TypedResults.Created($"/paiement-client/{result.Value}", request);
    }
}

