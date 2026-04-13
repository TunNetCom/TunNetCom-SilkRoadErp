using Microsoft.AspNetCore.Http.HttpResults;
using TunNetCom.SilkRoadErp.Sales.Contracts.CompteBancaire;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.CompteBancaire.UpdateCompteBancaire;

public class UpdateCompteBancaireEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPut("/compte-bancaire/{id:int}", HandleUpdateAsync)
            .WithTags(EndpointTags.CompteBancaire);
    }

    public async Task<Results<NoContent, ValidationProblem, NotFound>> HandleUpdateAsync(
        IMediator mediator,
        int id,
        UpdateCompteBancaireRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateCompteBancaireCommand(
            id,
            request.BanqueId,
            request.CodeEtablissement,
            request.CodeAgence,
            request.NumeroCompte,
            request.CleRib,
            request.Libelle);
        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            if (result.Errors.Any(e => e.Message == "compte_bancaire_not_found"))
            {
                return TypedResults.NotFound();
            }
            return result.ToValidationProblem();
        }

        return TypedResults.NoContent();
    }
}
