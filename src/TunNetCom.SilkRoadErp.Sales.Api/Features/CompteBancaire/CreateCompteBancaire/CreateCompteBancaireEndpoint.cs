using TunNetCom.SilkRoadErp.Sales.Contracts.CompteBancaire;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.CompteBancaire.CreateCompteBancaire;

public class CreateCompteBancaireEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/compte-bancaire", HandleCreateAsync)
            .WithTags(EndpointTags.CompteBancaire);
    }

    public async Task<Results<Created<CreateCompteBancaireRequest>, ValidationProblem>> HandleCreateAsync(
        IMediator mediator,
        CreateCompteBancaireRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateCompteBancaireCommand(
            request.BanqueId,
            request.CodeEtablissement,
            request.CodeAgence,
            request.NumeroCompte,
            request.CleRib,
            request.Libelle);
        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return result.ToValidationProblem();
        }

        return TypedResults.Created($"/compte-bancaire/{result.Value}", request);
    }
}
