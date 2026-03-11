using FluentResults;
using MediatR;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Inventaire.CreateInventaire;

public record CreateInventaireCommand(
    int AccountingYearId,
    DateTime DateInventaire,
    string? Description,
    List<CreateLigneInventaireCommand> Lignes
) : IRequest<Result<int>>;

public record CreateLigneInventaireCommand(
    string RefProduit,
    int QuantiteReelle,
    decimal PrixHt,
    decimal DernierPrixAchat
);

