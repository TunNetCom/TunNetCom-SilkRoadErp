using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.priceQuote.ValidateQuotations;

public class ValidateQuotationsCommandHandler(
    SalesContext _context,
    ILogger<ValidateQuotationsCommandHandler> _logger)
    : IRequestHandler<ValidateQuotationsCommand, Result>
{
    public async Task<Result> Handle(ValidateQuotationsCommand command, CancellationToken cancellationToken)
    {
        if (command.Ids == null || !command.Ids.Any())
        {
            return Result.Fail("No ids provided");
        }

        var quotations = await _context.Devis
            .Where(d => command.Ids.Contains(d.Num))
            .ToListAsync(cancellationToken);

        if (quotations.Count != command.Ids.Count)
        {
            var foundIds = quotations.Select(d => d.Num).ToList();
            var missingIds = command.Ids.Except(foundIds).ToList();
            _logger.LogWarning("Quotations not found: {Ids}", string.Join(", ", missingIds));
            return Result.Fail($"quotations_not_found: {string.Join(", ", missingIds)}");
        }

        var errors = new List<string>();

        foreach (var quotation in quotations)
        {
            try
            {
                if (quotation.Statut == DocumentStatus.Draft)
                {
                    quotation.Valider();
                    // Marquer explicitement la propriété comme modifiée car le setter est privé
                    _context.Entry(quotation).Property(x => x.Statut).IsModified = true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Cannot validate quotation {Id}", quotation.Num);
                errors.Add($"Id {quotation.Num}: {ex.Message}");
            }
        }

        if (errors.Any())
        {
            return Result.Fail(errors);
        }

        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Validated {Count} quotations", quotations.Count);

        return Result.Ok();
    }
}