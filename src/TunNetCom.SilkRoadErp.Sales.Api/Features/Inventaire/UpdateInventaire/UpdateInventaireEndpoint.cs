using FluentResults;
using TunNetCom.SilkRoadErp.Sales.Contracts.Inventaire;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Inventaire.UpdateInventaire;

public class UpdateInventaireEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPut(
            "/inventaires/{id:int}",
            async (IMediator mediator, int id, UpdateInventaireRequest request, CancellationToken cancellationToken) =>
            {
                var command = new UpdateInventaireCommand(
                    Id: id,
                    DateInventaire: request.DateInventaire,
                    Description: request.Description,
                    Lignes: request.Lignes.Select(l => new UpdateLigneInventaireCommand(
                        Id: l.Id,
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
                return Results.NoContent();
            })
            .WithTags("Inventaire");
    }
}

