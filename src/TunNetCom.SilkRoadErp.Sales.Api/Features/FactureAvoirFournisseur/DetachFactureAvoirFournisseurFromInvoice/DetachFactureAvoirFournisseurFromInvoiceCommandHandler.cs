using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.ResultExtensions;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.FactureAvoirFournisseur.DetachFactureAvoirFournisseurFromInvoice;

public class DetachFactureAvoirFournisseurFromInvoiceCommandHandler(
    SalesContext _context,
    ILogger<DetachFactureAvoirFournisseurFromInvoiceCommandHandler> _logger)
    : IRequestHandler<DetachFactureAvoirFournisseurFromInvoiceCommand, Result>
{
    public async Task<Result> Handle(DetachFactureAvoirFournisseurFromInvoiceCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Detaching facture avoir fournisseur {FactureAvoirIds} from invoice {InvoiceId}",
            string.Join(", ", command.FactureAvoirFournisseurIds), command.FactureFournisseurId);

        // Verify invoice exists and get its Id
        var invoice = await _context.FactureFournisseur
            .FirstOrDefaultAsync(f => f.Num == command.FactureFournisseurId, cancellationToken);

        if (invoice == null)
        {
            _logger.LogWarning("FactureFournisseur with Num {Num} not found", command.FactureFournisseurId);
            return Result.Fail(EntityNotFound.Error());
        }

        // Get facture avoir fournisseurs that are linked to this invoice - use Id, not Num
        var factureAvoirs = await _context.FactureAvoirFournisseur
            .Where(f => command.FactureAvoirFournisseurIds.Contains(f.Num) &&
                       f.NumFactureFournisseur == invoice.Id)
            .ToListAsync(cancellationToken);

        if (factureAvoirs.Count != command.FactureAvoirFournisseurIds.Count)
        {
            var foundIds = factureAvoirs.Select(f => f.Num).ToList();
            var missingIds = command.FactureAvoirFournisseurIds.Except(foundIds).ToList();
            _logger.LogWarning("Some facture avoir fournisseur not found or not linked to this invoice: {Ids}", string.Join(", ", missingIds));
            return Result.Fail($"facture_avoir_fournisseur_not_found_or_not_linked: {string.Join(", ", missingIds)}");
        }

        // Check if any facture avoir is validated (cannot be modified)
        var validated = factureAvoirs
            .Where(f => f.Statut == DocumentStatus.Valid)
            .ToList();

        if (validated.Any())
        {
            var validatedIds = validated.Select(f => f.Num).ToList();
            _logger.LogWarning("FactureAvoirFournisseur is validated and cannot be modified: {Ids}", string.Join(", ", validatedIds));
            return Result.Fail($"facture_avoir_fournisseur_validated: {string.Join(", ", validatedIds)}");
        }

        // Detach facture avoir from invoice
        foreach (var factureAvoir in factureAvoirs)
        {
            factureAvoir.NumFactureFournisseur = null;
        }

        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Successfully detached {Count} facture avoir fournisseur from invoice {InvoiceId}",
            factureAvoirs.Count, command.FactureFournisseurId);

        return Result.Ok();
    }
}

