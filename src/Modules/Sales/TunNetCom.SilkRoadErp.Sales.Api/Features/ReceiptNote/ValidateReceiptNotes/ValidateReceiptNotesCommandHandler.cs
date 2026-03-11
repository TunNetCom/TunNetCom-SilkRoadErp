using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ReceiptNote.ValidateReceiptNotes;

public class ValidateReceiptNotesCommandHandler(
    SalesContext _context,
    ILogger<ValidateReceiptNotesCommandHandler> _logger)
    : IRequestHandler<ValidateReceiptNotesCommand, Result>
{
    public async Task<Result> Handle(ValidateReceiptNotesCommand command, CancellationToken cancellationToken)
    {
        if (command.Ids == null || !command.Ids.Any())
        {
            return Result.Fail("No ids provided");
        }

        var receiptNotes = await _context.BonDeReception
            .AsTracking() // S'assurer que les entités sont trackées
            .Where(b => command.Ids.Contains(b.Num))
            .ToListAsync(cancellationToken);

        if (receiptNotes.Count != command.Ids.Count)
        {
            var foundIds = receiptNotes.Select(b => b.Num).ToList();
            var missingIds = command.Ids.Except(foundIds).ToList();
            _logger.LogWarning("ReceiptNotes not found: {Ids}", string.Join(", ", missingIds));
            return Result.Fail($"receipt_notes_not_found: {string.Join(", ", missingIds)}");
        }

        var errors = new List<string>();

        foreach (var receiptNote in receiptNotes)
        {
            try
            {
                if (receiptNote.Statut == DocumentStatus.Draft)
                {
                    receiptNote.Valider();
                    // Marquer explicitement la propriété comme modifiée car le setter est privé
                    var entry = _context.Entry(receiptNote);
                    entry.Property(x => x.Statut).IsModified = true;

                    // Forcer l'état de l'entité à Modified pour s'assurer que les changements sont sauvegardés
                    if (entry.State == Microsoft.EntityFrameworkCore.EntityState.Unchanged)
                    {
                        entry.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    }

                    _logger.LogInformation("Marked Statut as modified for receipt note {Num}. Current status: {Statut}, Entry state: {State}",
                        receiptNote.Num, receiptNote.Statut, entry.State);
                }
                else
                {
                    _logger.LogWarning("Receipt note {Num} is not in draft status (current: {Statut}), skipping validation",
                        receiptNote.Num, receiptNote.Statut);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Cannot validate receipt note {Id}", receiptNote.Num);
                errors.Add($"Id {receiptNote.Num}: {ex.Message}");
            }
        }

        if (errors.Any())
        {
            return Result.Fail(errors);
        }

        var savedChanges = await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Saved {SavedChanges} changes. Validated {Count} receipt notes", savedChanges, receiptNotes.Count);

        // Vérifier que les statuts ont bien été mis à jour
        foreach (var receiptNote in receiptNotes)
        {
            _logger.LogInformation("Receipt note {Num} status after save: {Statut}", receiptNote.Num, receiptNote.Statut);
        }

        return Result.Ok();
    }
}