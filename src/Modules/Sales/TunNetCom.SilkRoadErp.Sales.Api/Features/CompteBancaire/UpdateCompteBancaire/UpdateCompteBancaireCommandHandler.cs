namespace TunNetCom.SilkRoadErp.Sales.Api.Features.CompteBancaire.UpdateCompteBancaire;

public class UpdateCompteBancaireCommandHandler(
    SalesContext _context,
    ILogger<UpdateCompteBancaireCommandHandler> _logger)
    : IRequestHandler<UpdateCompteBancaireCommand, Result>
{
    public async Task<Result> Handle(UpdateCompteBancaireCommand command, CancellationToken cancellationToken)
    {
        var compte = await _context.CompteBancaire.FindAsync([command.Id], cancellationToken);
        if (compte == null)
        {
            return Result.Fail("compte_bancaire_not_found");
        }

        var banqueExists = await _context.Banque.AnyAsync(b => b.Id == command.BanqueId, cancellationToken);
        if (!banqueExists)
        {
            return Result.Fail("banque_not_found");
        }

        compte.UpdateCompteBancaire(
            command.BanqueId,
            command.CodeEtablissement,
            command.CodeAgence,
            command.NumeroCompte,
            command.CleRib,
            command.Libelle);

        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("CompteBancaire {Id} updated", command.Id);
        return Result.Ok();
    }
}
