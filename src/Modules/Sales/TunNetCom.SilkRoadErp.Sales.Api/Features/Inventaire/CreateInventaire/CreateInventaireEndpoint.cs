using FluentResults;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure;
using TunNetCom.SilkRoadErp.Sales.Contracts.Inventaire;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Inventaire.CreateInventaire;

public class CreateInventaireEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPost(
            "/inventaires",
            async Task<Results<Created<CreateInventaireRequest>, ValidationProblem>>
            (IMediator mediator,
            CreateInventaireRequest request, CancellationToken cancellationToken) =>
            {
                var command = new CreateInventaireCommand(
                    AccountingYearId: request.AccountingYearId,
                    DateInventaire: request.DateInventaire,
                    Description: request.Description,
                    Lignes: request.Lignes.Select(l => new CreateLigneInventaireCommand(
                        RefProduit: l.RefProduit,
                        QuantiteReelle: l.QuantiteReelle,
                        PrixHt: l.PrixHt,
                        DernierPrixAchat: l.DernierPrixAchat
                    )).ToList()
                );

                var result = await mediator.Send(command, cancellationToken);
                if (result.IsFailed)
                {
                    return result.ToValidationProblem();
                }
                return TypedResults.Created($"/inventaires/{result.Value}", request);
            })
            .WithTags("Inventaire");
    }
}

