using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.ResultExtensions;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.FactureAvoirFournisseur.AttachFactureAvoirFournisseurToInvoice;

public class AttachFactureAvoirFournisseurToInvoiceCommandHandler(
    SalesContext _context,
    ILogger<AttachFactureAvoirFournisseurToInvoiceCommandHandler> _logger)
    : IRequestHandler<AttachFactureAvoirFournisseurToInvoiceCommand, Result>
{
    public async Task<Result> Handle(AttachFactureAvoirFournisseurToInvoiceCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Attaching facture avoir fournisseur {FactureAvoirIds} to invoice {InvoiceId}",
            string.Join(", ", command.FactureAvoirFournisseurIds), command.FactureFournisseurId);

        // Verify invoice exists
        var invoice = await _context.FactureFournisseur
            .FirstOrDefaultAsync(f => f.Num == command.FactureFournisseurId, cancellationToken);

        if (invoice == null)
        {
            _logger.LogWarning("FactureFournisseur with Num {Num} not found", command.FactureFournisseurId);
            return Result.Fail(EntityNotFound.Error());
        }

        // Get facture avoir fournisseurs
        var factureAvoirs = await _context.FactureAvoirFournisseur
            .Where(f => command.FactureAvoirFournisseurIds.Contains(f.Num))
            .ToListAsync(cancellationToken);

        if (factureAvoirs.Count != command.FactureAvoirFournisseurIds.Count)
        {
            var foundIds = factureAvoirs.Select(f => f.Num).ToList();
            var missingIds = command.FactureAvoirFournisseurIds.Except(foundIds).ToList();
            _logger.LogWarning("FactureAvoirFournisseur not found: {Ids}", string.Join(", ", missingIds));
            return Result.Fail($"facture_avoir_fournisseur_not_found: {string.Join(", ", missingIds)}");
        }

        // Verify all facture avoir belong to the same provider as the invoice
        var invoiceProviderId = invoice.IdFournisseur;
        var mismatchedProviders = factureAvoirs
            .Where(f => f.IdFournisseur != invoiceProviderId)
            .ToList();

        if (mismatchedProviders.Any())
        {
            var mismatchedIds = mismatchedProviders.Select(f => f.Num).ToList();
            _logger.LogWarning("FactureAvoirFournisseur with different provider: {Ids}", string.Join(", ", mismatchedIds));
            return Result.Fail($"facture_avoir_fournisseur_provider_mismatch: {string.Join(", ", mismatchedIds)}");
        }

        // Check if any facture avoir is already linked to another invoice
        var alreadyLinked = factureAvoirs
            .Where(f => f.NumFactureFournisseur.HasValue && f.NumFactureFournisseur.Value != command.FactureFournisseurId)
            .ToList();

        if (alreadyLinked.Any())
        {
            var linkedIds = alreadyLinked.Select(f => f.Num).ToList();
            _logger.LogWarning("FactureAvoirFournisseur already linked to another invoice: {Ids}", string.Join(", ", linkedIds));
            return Result.Fail($"facture_avoir_fournisseur_already_linked: {string.Join(", ", linkedIds)}");
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

        // Attach facture avoir to invoice
        foreach (var factureAvoir in factureAvoirs)
        {
            factureAvoir.NumFactureFournisseur = command.FactureFournisseurId;
        }

        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Successfully attached {Count} facture avoir fournisseur to invoice {InvoiceId}",
            factureAvoirs.Count, command.FactureFournisseurId);

        return Result.Ok();
    }
}

