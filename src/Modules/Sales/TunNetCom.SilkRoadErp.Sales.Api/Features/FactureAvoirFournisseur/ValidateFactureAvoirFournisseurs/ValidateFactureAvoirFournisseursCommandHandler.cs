using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.FactureAvoirFournisseur.ValidateFactureAvoirFournisseurs;

public class ValidateFactureAvoirFournisseursCommandHandler(
    SalesContext _context,
    ILogger<ValidateFactureAvoirFournisseursCommandHandler> _logger)
    : IRequestHandler<ValidateFactureAvoirFournisseursCommand, Result>
{
    public async Task<Result> Handle(ValidateFactureAvoirFournisseursCommand command, CancellationToken cancellationToken)
    {
        if (command.Ids == null || !command.Ids.Any())
        {
            return Result.Fail("No ids provided");
        }

        var factureAvoirFournisseurs = await _context.FactureAvoirFournisseur
            .Where(f => command.Ids.Contains(f.Id))
            .ToListAsync(cancellationToken);

        if (factureAvoirFournisseurs.Count != command.Ids.Count)
        {
            var foundIds = factureAvoirFournisseurs.Select(f => f.Id).ToList();
            var missingIds = command.Ids.Except(foundIds).ToList();
            _logger.LogWarning("FactureAvoirFournisseurs not found: {Ids}", string.Join(", ", missingIds));
            return Result.Fail($"facture_avoir_fournisseurs_not_found: {string.Join(", ", missingIds)}");
        }

        var errors = new List<string>();

        foreach (var facture in factureAvoirFournisseurs)
        {
            try
            {
                if (facture.Statut == DocumentStatus.Draft)
                {
                    facture.Valider();
                    // Marquer explicitement la propriété comme modifiée car le setter est privé
                    _context.Entry(facture).Property(x => x.Statut).IsModified = true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Cannot validate facture avoir fournisseur {Id}", facture.Id);
                errors.Add($"Id {facture.Id}: {ex.Message}");
            }
        }

        if (errors.Any())
        {
            return Result.Fail(errors);
        }

        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Validated {Count} facture avoir fournisseurs", factureAvoirFournisseurs.Count);

        return Result.Ok();
    }
}