using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services.DocumentStorage;
using TunNetCom.SilkRoadErp.Sales.Contracts.PaiementClient;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.PaiementClient.UpdatePaiementClient;

public class UpdatePaiementClientCommandHandler(
    SalesContext _context,
    ILogger<UpdatePaiementClientCommandHandler> _logger,
    IDocumentStorageService _documentStorageService)
    : IRequestHandler<UpdatePaiementClientCommand, Result>
{
    public async Task<Result> Handle(UpdatePaiementClientCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("UpdatePaiementClientCommand called with Id {Id}", command.Id);

        var paiement = await _context.PaiementClient
            .Include(p => p.Factures)
            .Include(p => p.BonDeLivraisons)
            .FirstOrDefaultAsync(p => p.Id == command.Id, cancellationToken);

        if (paiement == null)
        {
            return Result.Fail("paiement_client_not_found");
        }

        // Validate numero is unique (if changed and provided)
        if (paiement.NumeroTransactionBancaire != command.NumeroTransactionBancaire && !string.IsNullOrWhiteSpace(command.NumeroTransactionBancaire))
        {
            var numeroExists = await _context.PaiementClient
                .AnyAsync(p => p.NumeroTransactionBancaire == command.NumeroTransactionBancaire && p.Id != command.Id, cancellationToken);
            if (numeroExists)
            {
                return Result.Fail("numero_already_exists");
            }
        }

        // Validate MethodePaiement enum
        if (!Enum.TryParse<MethodePaiement>(command.MethodePaiement, out var methodePaiement))
        {
            return Result.Fail("invalid_methode_paiement");
        }

        // Resolve accounting year: use requested one if provided and exists, otherwise active
        var accountingYear = command.AccountingYearId.HasValue
            ? await _context.AccountingYear.FirstOrDefaultAsync(ay => ay.Id == command.AccountingYearId.Value, cancellationToken)
            : await _context.AccountingYear.FirstOrDefaultAsync(ay => ay.IsActive, cancellationToken);

        if (accountingYear == null)
        {
            return Result.Fail("no_active_accounting_year");
        }

        // Validate exclusivity: either FactureIds or BonDeLivraisonIds, but not both
        var hasFactures = command.FactureIds != null && command.FactureIds.Count > 0;
        var hasBonDeLivraisons = command.BonDeLivraisonIds != null && command.BonDeLivraisonIds.Count > 0;
        
        if (hasFactures && hasBonDeLivraisons)
        {
            return Result.Fail("cannot_have_both_factures_and_bon_de_livraisons");
        }

        // Validate document links if provided
        if (hasFactures)
        {
            var factureIds = command.FactureIds!.Distinct().ToList();
            var facturesExist = await _context.Facture
                .Where(f => f.AccountingYearId == accountingYear.Id && factureIds.Contains(f.Id))
                .Select(f => f.Id)
                .ToListAsync(cancellationToken);
            
            var missingFactures = factureIds.Except(facturesExist).ToList();
            if (missingFactures.Any())
            {
                return Result.Fail($"factures_not_found: {string.Join(", ", missingFactures)}");
            }
        }

        if (hasBonDeLivraisons)
        {
            var bonDeLivraisonIds = command.BonDeLivraisonIds!.Distinct().ToList();
            var bonDeLivraisonsExist = await _context.BonDeLivraison
                .Where(b => b.AccountingYearId == accountingYear.Id && bonDeLivraisonIds.Contains(b.Id))
                .Select(b => b.Id)
                .ToListAsync(cancellationToken);
            
            var missingBonDeLivraisons = bonDeLivraisonIds.Except(bonDeLivraisonsExist).ToList();
            if (missingBonDeLivraisons.Any())
            {
                return Result.Fail($"bon_de_livraisons_not_found: {string.Join(", ", missingBonDeLivraisons)}");
            }
        }

        // Validate banque if provided
        if (command.BanqueId.HasValue)
        {
            var banqueExists = await _context.Banque.AnyAsync(b => b.Id == command.BanqueId.Value, cancellationToken);
            if (!banqueExists)
            {
                return Result.Fail("banque_not_found");
            }
        }

        // Process document if provided
        string? documentStoragePath = paiement.DocumentStoragePath; // Keep existing if no new document
        if (!string.IsNullOrWhiteSpace(command.DocumentBase64))
        {
            try
            {
                // Delete old document if exists
                if (!string.IsNullOrWhiteSpace(paiement.DocumentStoragePath))
                {
                    try
                    {
                        await _documentStorageService.DeleteAsync(paiement.DocumentStoragePath, cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Error deleting old document, continuing with new document");
                    }
                }

                var documentBytes = Convert.FromBase64String(command.DocumentBase64);
                var fileName = $"paiement_client_{command.NumeroTransactionBancaire}_{DateTime.UtcNow:yyyyMMddHHmmss}";
                documentStoragePath = await _documentStorageService.SaveAsync(documentBytes, fileName, cancellationToken);
            }
            catch (FormatException ex)
            {
                _logger.LogError(ex, "Invalid Base64 format for document");
                return Result.Fail("invalid_document_format");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error storing document");
                return Result.Fail("error_storing_document");
            }
        }

        paiement.UpdatePaiementClient(
            command.NumeroTransactionBancaire,
            command.ClientId,
            accountingYear.Id,
            command.Montant,
            command.DatePaiement,
            methodePaiement,
            command.FactureIds,
            command.BonDeLivraisonIds,
            command.NumeroChequeTraite,
            command.BanqueId,
            command.DateEcheance,
            command.Commentaire,
            documentStoragePath);

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("PaiementClient with Id {Id} updated successfully", command.Id);
        return Result.Ok();
    }
}

