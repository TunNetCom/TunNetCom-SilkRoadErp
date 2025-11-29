using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Avoirs.ValidateAvoirs;

public class ValidateAvoirsCommandHandler(
    SalesContext _context,
    ILogger<ValidateAvoirsCommandHandler> _logger)
    : IRequestHandler<ValidateAvoirsCommand, Result>
{
    public async Task<Result> Handle(ValidateAvoirsCommand command, CancellationToken cancellationToken)
    {
        if (command.Ids == null || !command.Ids.Any())
        {
            return Result.Fail("No ids provided");
        }

        var avoirs = await _context.Avoirs
            .Where(a => command.Ids.Contains(a.Num))
            .ToListAsync(cancellationToken);

        if (avoirs.Count != command.Ids.Count)
        {
            var foundIds = avoirs.Select(a => a.Num).ToList();
            var missingIds = command.Ids.Except(foundIds).ToList();
            _logger.LogWarning("Avoirs not found: {Ids}", string.Join(", ", missingIds));
            return Result.Fail($"avoirs_not_found: {string.Join(", ", missingIds)}");
        }

        var errors = new List<string>();

        foreach (var avoir in avoirs)
        {
            try
            {
                if (avoir.Statut == DocumentStatus.Brouillon)
                {
                    avoir.Valider();
                    // Marquer explicitement la propriété comme modifiée car le setter est privé
                    _context.Entry(avoir).Property(x => x.Statut).IsModified = true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Cannot validate avoir {Id}", avoir.Num);
                errors.Add($"Id {avoir.Num}: {ex.Message}");
            }
        }

        if (errors.Any())
        {
            return Result.Fail(errors);
        }

        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Validated {Count} avoirs", avoirs.Count);

        return Result.Ok();
    }
}


