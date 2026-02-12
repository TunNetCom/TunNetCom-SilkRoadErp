namespace TunNetCom.SilkRoadErp.Sales.Api.Features.TiersDepenseFonctionnement.UpdateTiersDepenseFonctionnement;

public record UpdateTiersDepenseFonctionnementCommand(
    int Id,
    string Nom,
    string? Tel,
    string? Adresse,
    string? Matricule,
    string? Code,
    string? CodeCat,
    string? EtbSec,
    string? Mail) : IRequest<Result>;
