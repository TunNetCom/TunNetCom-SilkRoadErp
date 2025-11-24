using TunNetCom.SilkRoadErp.Sales.Contracts.PaiementClient;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.PaiementClient.UpdatePaiementClient;

public class UpdatePaiementClientEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPut("/paiement-client/{id:int}", HandleUpdatePaiementClientAsync)
            .WithTags(EndpointTags.PaiementClient);
    }

    public async Task<Results<NoContent, ValidationProblem>> HandleUpdatePaiementClientAsync(
        IMediator mediator,
        int id,
        UpdatePaiementClientRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdatePaiementClientCommand(
            id,
            request.Numero,
            request.ClientId,
            request.Montant,
            request.DatePaiement,
            request.MethodePaiement,
            request.FactureId,
            request.BonDeLivraisonId,
            request.NumeroChequeTraite,
            request.BanqueId,
            request.DateEcheance,
            request.Commentaire);

        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return result.ToValidationProblem();
        }

        return TypedResults.NoContent();
    }
}

