using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.CompteBancaire.CreateCompteBancaire;

public class CreateCompteBancaireCommandHandler(
    SalesContext _context,
    ILogger<CreateCompteBancaireCommandHandler> _logger)
    : IRequestHandler<CreateCompteBancaireCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateCompteBancaireCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("CreateCompteBancaireCommand called for BanqueId {BanqueId}", command.BanqueId);

        var banqueExists = await _context.Banque.AnyAsync(b => b.Id == command.BanqueId, cancellationToken);
        if (!banqueExists)
        {
            return Result.Fail("banque_not_found");
        }

        var compte = Domain.Entites.CompteBancaire.CreateCompteBancaire(
            command.BanqueId,
            command.CodeEtablissement,
            command.CodeAgence,
            command.NumeroCompte,
            command.CleRib,
            command.Libelle);
        _context.CompteBancaire.Add(compte);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("CompteBancaire created with Id {Id}", compte.Id);
        return Result.Ok(compte.Id);
    }
}
