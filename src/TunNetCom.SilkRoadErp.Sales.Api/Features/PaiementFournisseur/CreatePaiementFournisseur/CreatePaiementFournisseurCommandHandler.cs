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
        _logger.LogInformation("CreatePaiementFournisseurCommand called with FournisseurId {FournisseurId} and Numero {Numero}", command.FournisseurId, command.Numero);

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

        // Validate numero is unique
        var numeroExists = await _context.PaiementFournisseur
            .AnyAsync(p => p.Numero == command.Numero, cancellationToken);
        if (numeroExists)
        {
            return Result.Fail("numero_already_exists");
        }

        // Validate MethodePaiement enum
        if (!Enum.TryParse<MethodePaiement>(command.MethodePaiement, out var methodePaiement))
        {
            return Result.Fail("invalid_methode_paiement");
        }

        // Validate document links if provided
        if (command.FactureFournisseurId.HasValue)
        {
            var factureExists = await _context.FactureFournisseur.AnyAsync(f => f.Id == command.FactureFournisseurId.Value, cancellationToken);
            if (!factureExists)
            {
                return Result.Fail("facture_fournisseur_not_found");
            }
        }

        if (command.BonDeReceptionId.HasValue)
        {
            var bonDeReceptionExists = await _context.BonDeReception.AnyAsync(b => b.Id == command.BonDeReceptionId.Value, cancellationToken);
            if (!bonDeReceptionExists)
            {
                return Result.Fail("bon_de_reception_not_found");
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
                var fileName = $"paiement_fournisseur_{command.Numero}_{DateTime.UtcNow:yyyyMMddHHmmss}";
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
            command.Numero,
            command.FournisseurId,
            activeAccountingYear.Id,
            command.Montant,
            command.DatePaiement,
            methodePaiement,
            command.FactureFournisseurId,
            command.BonDeReceptionId,
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

        _logger.LogInformation("PaiementFournisseur created successfully with Id {Id}", paiement.Id);
        return Result.Ok(paiement.Id);
    }
}

