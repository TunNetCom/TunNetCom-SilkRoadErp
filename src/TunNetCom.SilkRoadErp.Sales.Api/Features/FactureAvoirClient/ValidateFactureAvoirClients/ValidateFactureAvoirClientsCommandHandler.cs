using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.FactureAvoirClient.ValidateFactureAvoirClients;

public class ValidateFactureAvoirClientsCommandHandler(
    SalesContext _context,
    ILogger<ValidateFactureAvoirClientsCommandHandler> _logger)
    : IRequestHandler<ValidateFactureAvoirClientsCommand, Result>
{
    public async Task<Result> Handle(ValidateFactureAvoirClientsCommand command, CancellationToken cancellationToken)
    {
        if (command.Ids == null || !command.Ids.Any())
        {
            return Result.Fail("No ids provided");
        }

        var factureAvoirClients = await _context.FactureAvoirClient
            .Where(f => command.Ids.Contains(f.Num))
            .ToListAsync(cancellationToken);

        if (factureAvoirClients.Count != command.Ids.Count)
        {
            var foundIds = factureAvoirClients.Select(f => f.Num).ToList();
            var missingIds = command.Ids.Except(foundIds).ToList();
            _logger.LogWarning("FactureAvoirClients not found: {Ids}", string.Join(", ", missingIds));
            return Result.Fail($"facture_avoir_clients_not_found: {string.Join(", ", missingIds)}");
        }

        var errors = new List<string>();

        foreach (var facture in factureAvoirClients)
        {
            try
            {
                if (facture.Statut == DocumentStatus.Brouillon)
                {
                    facture.Valider();
                    // Marquer explicitement la propriété comme modifiée car le setter est privé
                    _context.Entry(facture).Property(x => x.Statut).IsModified = true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Cannot validate facture avoir client {Id}", facture.Num);
                errors.Add($"Id {facture.Num}: {ex.Message}");
            }
        }

        if (errors.Any())
        {
            return Result.Fail(errors);
        }

        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Validated {Count} facture avoir clients", factureAvoirClients.Count);

        return Result.Ok();
    }
}