using TunNetCom.SilkRoadErp.Sales.Contracts.TiersDepenseFonctionnement;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.TiersDepenseFonctionnement.CreateTiersDepenseFonctionnement;

public record CreateTiersDepenseFonctionnementCommand(
    string Nom,
    string? Tel,
    string? Adresse,
    string? Matricule,
    string? Code,
    string? CodeCat,
    string? EtbSec,
    string? Mail,
    bool ExonereRetenueSource = false) : IRequest<Result<int>>;
