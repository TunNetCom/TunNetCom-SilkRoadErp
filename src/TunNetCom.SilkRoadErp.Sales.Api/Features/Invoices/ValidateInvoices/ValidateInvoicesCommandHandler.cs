using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Invoices.ValidateInvoices;

public class ValidateInvoicesCommandHandler(
    SalesContext _context,
    ILogger<ValidateInvoicesCommandHandler> _logger)
    : IRequestHandler<ValidateInvoicesCommand, Result>
{
    public async Task<Result> Handle(ValidateInvoicesCommand command, CancellationToken cancellationToken)
    {
        if (command.Ids == null || !command.Ids.Any())
        {
            return Result.Fail("No ids provided");
        }

        var invoices = await _context.Facture
            .Where(f => command.Ids.Contains(f.Num))
            .ToListAsync(cancellationToken);

        if (invoices.Count != command.Ids.Count)
        {
            var foundIds = invoices.Select(f => f.Num).ToList();
            var missingIds = command.Ids.Except(foundIds).ToList();
            _logger.LogWarning("Invoices not found: {Ids}", string.Join(", ", missingIds));
            return Result.Fail($"invoices_not_found: {string.Join(", ", missingIds)}");
        }

        var errors = new List<string>();

        foreach (var invoice in invoices)
        {
            try
            {
                if (invoice.Statut == DocumentStatus.Brouillon)
                {
                    invoice.Valider();
                    // Marquer explicitement la propriété comme modifiée car le setter est privé
                    _context.Entry(invoice).Property(x => x.Statut).IsModified = true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Cannot validate invoice {Id}", invoice.Num);
                errors.Add($"Id {invoice.Num}: {ex.Message}");
            }
        }

        if (errors.Any())
        {
            return Result.Fail(errors);
        }

        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Validated {Count} invoices", invoices.Count);

        return Result.Ok();
    }
}