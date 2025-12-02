using TunNetCom.SilkRoadErp.Sales.Contracts.FactureAvoirFournisseur;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.FactureAvoirFournisseur.UpdateFactureAvoirFournisseur;

public class UpdateFactureAvoirFournisseurCommandHandler(
    SalesContext _context,
    ILogger<UpdateFactureAvoirFournisseurCommandHandler> _logger)
    : IRequestHandler<UpdateFactureAvoirFournisseurCommand, Result>
{
    public async Task<Result> Handle(UpdateFactureAvoirFournisseurCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("UpdateFactureAvoirFournisseurCommand called with Num {Num}", command.Num);

        var factureAvoirFournisseur = await _context.FactureAvoirFournisseur
            .Include(f => f.AvoirFournisseur)
            .FirstOrDefaultAsync(f => f.Num == command.Num, cancellationToken);

        if (factureAvoirFournisseur == null)
        {
            _logger.LogEntityNotFound(nameof(FactureAvoirFournisseur), command.Num);
            return Result.Fail(EntityNotFound.Error());
        }

        if (factureAvoirFournisseur.Statut == DocumentStatus.Valid)
        {
            return Result.Fail("Le document est validé et ne peut plus être modifié.");
        }

        var fournisseurExists = await _context.Fournisseur.AnyAsync(f => f.Id == command.IdFournisseur, cancellationToken);
        if (!fournisseurExists)
        {
            return Result.Fail("fournisseur_not_found");
        }

        if (command.NumFactureFournisseur.HasValue)
        {
            var factureFournisseurExists = await _context.FactureFournisseur.AnyAsync(f => f.Num == command.NumFactureFournisseur.Value, cancellationToken);
            if (!factureFournisseurExists)
            {
                return Result.Fail("facture_fournisseur_not_found");
            }
        }

        // Unlink existing avoir fournisseurs
        foreach (var existingAvoir in factureAvoirFournisseur.AvoirFournisseur)
        {
            existingAvoir.NumFactureAvoirFournisseur = null;
        }

        // Validate new avoir fournisseurs exist and are not already linked to another facture avoir
        var avoirFournisseurs = await _context.AvoirFournisseur
            .Where(a => command.AvoirFournisseurIds.Contains(a.Num))
            .ToListAsync(cancellationToken);

        if (avoirFournisseurs.Count != command.AvoirFournisseurIds.Count)
        {
            var foundIds = avoirFournisseurs.Select(a => a.Num).ToList();
            var missingIds = command.AvoirFournisseurIds.Except(foundIds).ToList();
            _logger.LogWarning("AvoirFournisseurs not found: {Ids}", string.Join(", ", missingIds));
            return Result.Fail($"avoir_fournisseurs_not_found: {string.Join(", ", missingIds)}");
        }

        // Check if any avoir fournisseur is already linked to another facture avoir
        var alreadyLinked = avoirFournisseurs
            .Where(a => a.NumFactureAvoirFournisseur.HasValue && a.NumFactureAvoirFournisseur.Value != command.Num)
            .ToList();
        if (alreadyLinked.Any())
        {
            var linkedIds = alreadyLinked.Select(a => a.Num).ToList();
            _logger.LogWarning("AvoirFournisseurs already linked to another facture: {Ids}", string.Join(", ", linkedIds));
            return Result.Fail($"avoir_fournisseurs_already_linked: {string.Join(", ", linkedIds)}");
        }

        // Get the active accounting year
        var activeAccountingYear = await _context.AccountingYear
            .FirstOrDefaultAsync(ay => ay.IsActive, cancellationToken);

        if (activeAccountingYear == null)
        {
            _logger.LogError("No active accounting year found");
            return Result.Fail("no_active_accounting_year");
        }

        // Update facture avoir fournisseur
        factureAvoirFournisseur.UpdateFactureAvoirFournisseur(
            factureAvoirFournisseur.NumFactureAvoirFourSurPage,
            command.IdFournisseur,
            command.Date,
            command.NumFactureFournisseur,
            activeAccountingYear.Id);

        // Link new avoir fournisseurs
        foreach (var avoirFournisseur in avoirFournisseurs)
        {
            avoirFournisseur.NumFactureAvoirFournisseur = command.Num;
        }

        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("FactureAvoirFournisseur with Num {Num} updated successfully", factureAvoirFournisseur.Num);
        return Result.Ok();
    }
}

