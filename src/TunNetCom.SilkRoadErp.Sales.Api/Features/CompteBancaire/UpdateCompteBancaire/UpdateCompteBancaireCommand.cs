using TunNetCom.SilkRoadErp.Sales.Contracts.CompteBancaire;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.CompteBancaire.UpdateCompteBancaire;

public record UpdateCompteBancaireCommand(
    int Id,
    int BanqueId,
    string CodeEtablissement,
    string CodeAgence,
    string NumeroCompte,
    string CleRib,
    string? Libelle) : IRequest<Result>;
