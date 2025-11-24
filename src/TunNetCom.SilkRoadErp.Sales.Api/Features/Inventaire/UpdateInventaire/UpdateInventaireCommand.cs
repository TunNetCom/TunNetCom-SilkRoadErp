using FluentResults;
using MediatR;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Inventaire.UpdateInventaire;

public record UpdateInventaireCommand(
    int Id,
    DateTime DateInventaire,
    string? Description,
    List<UpdateLigneInventaireCommand> Lignes
) : IRequest<Result>;

public record UpdateLigneInventaireCommand(
    int? Id,
    string RefProduit,
    int QuantiteReelle,
    decimal PrixHt,
    decimal DernierPrixAchat
);

