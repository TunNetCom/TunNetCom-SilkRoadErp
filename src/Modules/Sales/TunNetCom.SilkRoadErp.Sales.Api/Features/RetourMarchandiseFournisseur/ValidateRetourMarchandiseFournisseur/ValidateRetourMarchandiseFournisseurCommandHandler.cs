using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.RetourMarchandiseFournisseur.ValidateRetourMarchandiseFournisseur;

public class ValidateRetourMarchandiseFournisseurCommandHandler(
    SalesContext _context,
    ILogger<ValidateRetourMarchandiseFournisseurCommandHandler> _logger)
    : IRequestHandler<ValidateRetourMarchandiseFournisseurCommand, Result>
{
    public async Task<Result> Handle(
        ValidateRetourMarchandiseFournisseurCommand command,
        CancellationToken cancellationToken)
    {
        if (command.Ids == null || !command.Ids.Any())
        {
            return Result.Fail("No ids provided");
        }

        var retours = await _context.RetourMarchandiseFournisseur
            .AsTracking()
            .Where(r => command.Ids.Contains(r.Num))
            .ToListAsync(cancellationToken);

        if (retours.Count != command.Ids.Count)
        {
            var foundIds = retours.Select(r => r.Num).ToList();
            var missingIds = command.Ids.Except(foundIds).ToList();
            _logger.LogWarning("RetourMarchandiseFournisseur not found: {Ids}", string.Join(", ", missingIds));
            return Result.Fail($"retours_not_found: {string.Join(", ", missingIds)}");
        }

        var errors = new List<string>();

        foreach (var retour in retours)
        {
            try
            {
                // Utiliser StatutRetour au lieu de Statut (qui est ignoré par EF Core)
                if (retour.StatutRetour == RetourFournisseurStatus.Draft)
                {
                    retour.Valider();
                    var entry = _context.Entry(retour);
                    // Marquer StatutRetour comme modifié (pas Statut qui est ignoré)
                    entry.Property(x => x.StatutRetour).IsModified = true;

                    if (entry.State == Microsoft.EntityFrameworkCore.EntityState.Unchanged)
                    {
                        entry.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    }

                    _logger.LogInformation("Marked StatutRetour as modified for retour {Num}. Current status: {Statut}",
                        retour.Num, retour.StatutRetour);
                }
                else
                {
                    _logger.LogWarning("Retour {Num} is not in draft status (current: {Statut}), skipping validation",
                        retour.Num, retour.StatutRetour);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Cannot validate retour {Id}", retour.Num);
                errors.Add($"Id {retour.Num}: {ex.Message}");
            }
        }

        if (errors.Any())
        {
            return Result.Fail(errors);
        }

        var savedChanges = await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Saved {SavedChanges} changes. Validated {Count} retours", savedChanges, retours.Count);

        return Result.Ok();
    }
}

