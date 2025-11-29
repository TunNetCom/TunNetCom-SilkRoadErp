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
                if (receiptNote.Statut == DocumentStatus.Brouillon)
                {
                    receiptNote.Valider();
                    // Marquer explicitement la propriété comme modifiée car le setter est privé
                    _context.Entry(receiptNote).Property(x => x.Statut).IsModified = true;
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

        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Validated {Count} receipt notes", receiptNotes.Count);

        return Result.Ok();
    }
}


