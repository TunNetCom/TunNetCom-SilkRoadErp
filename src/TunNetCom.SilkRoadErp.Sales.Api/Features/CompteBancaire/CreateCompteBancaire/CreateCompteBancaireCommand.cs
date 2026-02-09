using TunNetCom.SilkRoadErp.Sales.Contracts.CompteBancaire;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.CompteBancaire.CreateCompteBancaire;

public record CreateCompteBancaireCommand(
    int BanqueId,
    string CodeEtablissement,
    string CodeAgence,
    string NumeroCompte,
    string CleRib,
    string? Libelle) : IRequest<Result<int>>;
