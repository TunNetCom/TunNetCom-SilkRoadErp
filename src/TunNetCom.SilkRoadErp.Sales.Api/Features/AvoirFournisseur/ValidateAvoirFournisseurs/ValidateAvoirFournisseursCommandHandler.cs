using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.AvoirFournisseur.ValidateAvoirFournisseurs;

public class ValidateAvoirFournisseursCommandHandler(
    SalesContext _context,
    ILogger<ValidateAvoirFournisseursCommandHandler> _logger)
    : IRequestHandler<ValidateAvoirFournisseursCommand, Result>
{
    public async Task<Result> Handle(ValidateAvoirFournisseursCommand command, CancellationToken cancellationToken)
    {
        if (command.Ids == null || !command.Ids.Any())
        {
            return Result.Fail("No ids provided");
        }

        var avoirFournisseurs = await _context.AvoirFournisseur
            .Where(a => command.Ids.Contains(a.Id))
            .ToListAsync(cancellationToken);

        if (avoirFournisseurs.Count != command.Ids.Count)
        {
            var foundIds = avoirFournisseurs.Select(a => a.Id).ToList();
            var missingIds = command.Ids.Except(foundIds).ToList();
            _logger.LogWarning("AvoirFournisseurs not found: {Ids}", string.Join(", ", missingIds));
            return Result.Fail($"avoir_fournisseurs_not_found: {string.Join(", ", missingIds)}");
        }

        var errors = new List<string>();

        foreach (var avoir in avoirFournisseurs)
        {
            try
            {
                if (avoir.Statut == DocumentStatus.Draft)
                {
                    avoir.Valider();
                    // Marquer explicitement la propriété comme modifiée car le setter est privé
                    _context.Entry(avoir).Property(x => x.Statut).IsModified = true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Cannot validate avoir fournisseur {Id}", avoir.Id);
                errors.Add($"Id {avoir.Id}: {ex.Message}");
            }
        }

        if (errors.Any())
        {
            return Result.Fail(errors);
        }

        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Validated {Count} avoir fournisseurs", avoirFournisseurs.Count);

        return Result.Ok();
    }
}