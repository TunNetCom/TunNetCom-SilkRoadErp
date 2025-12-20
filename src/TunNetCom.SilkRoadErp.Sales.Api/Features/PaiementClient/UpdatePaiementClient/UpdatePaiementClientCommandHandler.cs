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
            .FirstOrDefaultAsync(p => p.Id == command.Id, cancellationToken);

        if (paiement == null)
        {
            return Result.Fail("paiement_client_not_found");
        }

        // Validate numero is unique (if changed)
        if (paiement.Numero != command.Numero)
        {
            var numeroExists = await _context.PaiementClient
                .AnyAsync(p => p.Numero == command.Numero && p.Id != command.Id, cancellationToken);
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

        // Get active accounting year
        var activeAccountingYear = await _context.AccountingYear
            .FirstOrDefaultAsync(ay => ay.IsActive, cancellationToken);

        if (activeAccountingYear == null)
        {
            return Result.Fail("no_active_accounting_year");
        }

        // Validate document links if provided
        if (command.FactureId.HasValue)
        {
            var factureExists = await _context.Facture.AnyAsync(f => f.Id == command.FactureId.Value, cancellationToken);
            if (!factureExists)
            {
                return Result.Fail("facture_not_found");
            }
        }

        if (command.BonDeLivraisonId.HasValue)
        {
            var bonDeLivraisonExists = await _context.BonDeLivraison.AnyAsync(b => b.Id == command.BonDeLivraisonId.Value, cancellationToken);
            if (!bonDeLivraisonExists)
            {
                return Result.Fail("bon_de_livraison_not_found");
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
                var fileName = $"paiement_client_{command.Numero}_{DateTime.UtcNow:yyyyMMddHHmmss}";
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
            command.Numero,
            command.ClientId,
            activeAccountingYear.Id,
            command.Montant,
            command.DatePaiement,
            methodePaiement,
            command.FactureId,
            command.BonDeLivraisonId,
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

