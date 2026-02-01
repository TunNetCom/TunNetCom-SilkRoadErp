using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services.DocumentStorage;
using TunNetCom.SilkRoadErp.Sales.Contracts.PaiementClient;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.PaiementClient.CreatePaiementClient;

public class CreatePaiementClientCommandHandler(
    SalesContext _context,
    ILogger<CreatePaiementClientCommandHandler> _logger,
    IDocumentStorageService _documentStorageService)
    : IRequestHandler<CreatePaiementClientCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreatePaiementClientCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("CreatePaiementClientCommand called with ClientId {ClientId} and NumeroTransactionBancaire {NumeroTransactionBancaire}", command.ClientId, command.NumeroTransactionBancaire);

        // Validate client exists
        var clientExists = await _context.Client.AnyAsync(c => c.Id == command.ClientId, cancellationToken);
        if (!clientExists)
        {
            return Result.Fail("client_not_found");
        }

        // Resolve accounting year: use requested one if provided and exists, otherwise active
        var accountingYear = command.AccountingYearId.HasValue
            ? await _context.AccountingYear.FirstOrDefaultAsync(ay => ay.Id == command.AccountingYearId.Value, cancellationToken)
            : await _context.AccountingYear.FirstOrDefaultAsync(ay => ay.IsActive, cancellationToken);

        if (accountingYear == null)
        {
            _logger.LogError("Accounting year not found or no active accounting year");
            return Result.Fail("no_active_accounting_year");
        }

        // Validate numeroTransactionBancaire is unique (only if provided)
        if (!string.IsNullOrWhiteSpace(command.NumeroTransactionBancaire))
        {
            var numeroExists = await _context.PaiementClient
                .AnyAsync(p => p.NumeroTransactionBancaire == command.NumeroTransactionBancaire, cancellationToken);
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
        string? documentStoragePath = null;
        if (!string.IsNullOrWhiteSpace(command.DocumentBase64))
        {
            try
            {
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

        var paiement = Domain.Entites.PaiementClient.CreatePaiementClient(
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

        _context.PaiementClient.Add(paiement);
        await _context.SaveChangesAsync(cancellationToken);

        // Add junction entities after paiement has an ID
        if (hasFactures)
        {
            foreach (var factureId in command.FactureIds!)
            {
                paiement.Factures.Add(PaiementClientFacture.Create(paiement.Id, factureId));
            }
        }

        if (hasBonDeLivraisons)
        {
            foreach (var bonDeLivraisonId in command.BonDeLivraisonIds!)
            {
                paiement.BonDeLivraisons.Add(PaiementClientBonDeLivraison.Create(paiement.Id, bonDeLivraisonId));
            }
        }

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("PaiementClient created successfully with Id {Id}", paiement.Id);
        return Result.Ok(paiement.Id);
    }
}

