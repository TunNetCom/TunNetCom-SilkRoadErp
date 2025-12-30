using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services.DocumentStorage;
using TunNetCom.SilkRoadErp.Sales.Contracts.PaiementFournisseur;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.PaiementFournisseur.CreatePaiementFournisseur;

public class CreatePaiementFournisseurCommandHandler(
    SalesContext _context,
    ILogger<CreatePaiementFournisseurCommandHandler> _logger,
    IDocumentStorageService _documentStorageService)
    : IRequestHandler<CreatePaiementFournisseurCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreatePaiementFournisseurCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("CreatePaiementFournisseurCommand called with FournisseurId {FournisseurId} and NumeroTransactionBancaire {NumeroTransactionBancaire}", command.FournisseurId, command.NumeroTransactionBancaire);

        // Validate fournisseur exists
        var fournisseurExists = await _context.Fournisseur.AnyAsync(f => f.Id == command.FournisseurId, cancellationToken);
        if (!fournisseurExists)
        {
            return Result.Fail("fournisseur_not_found");
        }

        // Validate accounting year exists
        var activeAccountingYear = await _context.AccountingYear
            .FirstOrDefaultAsync(ay => ay.IsActive, cancellationToken);

        if (activeAccountingYear == null)
        {
            _logger.LogError("No active accounting year found");
            return Result.Fail("no_active_accounting_year");
        }

        // Validate numeroTransactionBancaire is unique (only if provided)
        if (!string.IsNullOrWhiteSpace(command.NumeroTransactionBancaire))
        {
            var numeroExists = await _context.PaiementFournisseur
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

        // Validate exclusivity: either FactureFournisseurIds or BonDeReceptionIds, but not both
        var hasFactures = command.FactureFournisseurIds != null && command.FactureFournisseurIds.Count > 0;
        var hasBonDeReceptions = command.BonDeReceptionIds != null && command.BonDeReceptionIds.Count > 0;
        
        if (hasFactures && hasBonDeReceptions)
        {
            return Result.Fail("cannot_have_both_factures_and_bon_de_receptions");
        }

        // Validate document links if provided
        if (hasFactures)
        {
            var factureIds = command.FactureFournisseurIds!.Distinct().ToList();
            var facturesExist = await _context.FactureFournisseur
                .Where(f => factureIds.Contains(f.Id))
                .Select(f => f.Id)
                .ToListAsync(cancellationToken);
            
            var missingFactures = factureIds.Except(facturesExist).ToList();
            if (missingFactures.Any())
            {
                return Result.Fail($"factures_fournisseur_not_found: {string.Join(", ", missingFactures)}");
            }
        }

        if (hasBonDeReceptions)
        {
            var bonDeReceptionIds = command.BonDeReceptionIds!.Distinct().ToList();
            var bonDeReceptionsExist = await _context.BonDeReception
                .Where(b => bonDeReceptionIds.Contains(b.Id))
                .Select(b => b.Id)
                .ToListAsync(cancellationToken);
            
            var missingBonDeReceptions = bonDeReceptionIds.Except(bonDeReceptionsExist).ToList();
            if (missingBonDeReceptions.Any())
            {
                return Result.Fail($"bon_de_receptions_not_found: {string.Join(", ", missingBonDeReceptions)}");
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
                var fileName = $"paiement_fournisseur_{command.NumeroTransactionBancaire}_{DateTime.UtcNow:yyyyMMddHHmmss}";
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

        var paiement = Domain.Entites.PaiementFournisseur.CreatePaiementFournisseur(
            command.NumeroTransactionBancaire,
            command.FournisseurId,
            activeAccountingYear.Id,
            command.Montant,
            command.DatePaiement,
            methodePaiement,
            command.FactureFournisseurIds,
            command.BonDeReceptionIds,
            command.NumeroChequeTraite,
            command.BanqueId,
            command.DateEcheance,
            command.Commentaire,
            command.RibCodeEtab,
            command.RibCodeAgence,
            command.RibNumeroCompte,
            command.RibCle,
            documentStoragePath);

        _context.PaiementFournisseur.Add(paiement);
        await _context.SaveChangesAsync(cancellationToken);

        // Add junction entities after paiement has an ID
        if (hasFactures)
        {
            foreach (var factureId in command.FactureFournisseurIds!)
            {
                var junction = PaiementFournisseurFactureFournisseur.Create(paiement.Id, factureId);
                paiement.FactureFournisseurs.Add(junction);
            }
        }

        if (hasBonDeReceptions)
        {
            foreach (var bonDeReceptionId in command.BonDeReceptionIds!)
            {
                var junction = PaiementFournisseurBonDeReception.Create(paiement.Id, bonDeReceptionId);
                paiement.BonDeReceptions.Add(junction);
            }
        }

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("PaiementFournisseur created successfully with Id {Id}", paiement.Id);

        return Result.Ok(paiement.Id);
    }
}

