using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.ValidateDeliveryNotes;

public class ValidateDeliveryNotesCommandHandler(
    SalesContext _context,
    ILogger<ValidateDeliveryNotesCommandHandler> _logger)
    : IRequestHandler<ValidateDeliveryNotesCommand, Result>
{
    public async Task<Result> Handle(ValidateDeliveryNotesCommand command, CancellationToken cancellationToken)
    {
        if (command.Ids == null || !command.Ids.Any())
        {
            return Result.Fail("No ids provided");
        }

        var deliveryNotes = await _context.BonDeLivraison
            .Where(b => command.Ids.Contains(b.Num))
            .ToListAsync(cancellationToken);

        if (deliveryNotes.Count != command.Ids.Count)
        {
            var foundNums = deliveryNotes.Select(b => b.Num).ToList();
            var missingNums = command.Ids.Except(foundNums).ToList();
            _logger.LogWarning("DeliveryNotes not found: {Nums}", string.Join(", ", missingNums));
            return Result.Fail($"delivery_notes_not_found: {string.Join(", ", missingNums)}");
        }

        var errors = new List<string>();

        foreach (var deliveryNote in deliveryNotes)
        {
            try
            {
                if (deliveryNote.Statut == DocumentStatus.Brouillon)
                {
                    deliveryNote.Valider();
                    // Marquer explicitement la propriété comme modifiée car le setter est privé
                    _context.Entry(deliveryNote).Property(x => x.Statut).IsModified = true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Cannot validate delivery note {Num}", deliveryNote.Num);
                errors.Add($"Num {deliveryNote.Num}: {ex.Message}");
            }
        }

        if (errors.Any())
        {
            return Result.Fail(errors);
        }

        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Validated {Count} delivery notes", deliveryNotes.Count);

        return Result.Ok();
    }
}


