using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.ResultExtensions;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services.DocumentStorage;
using Domain = TunNetCom.SilkRoadErp.Sales.Domain;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.PaiementTiersDepense.CreatePaiementTiersDepense;

public class CreatePaiementTiersDepenseCommandHandler(
    SalesContext _context,
    ILogger<CreatePaiementTiersDepenseCommandHandler> _logger,
    IDocumentStorageService _documentStorageService)
    : IRequestHandler<CreatePaiementTiersDepenseCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreatePaiementTiersDepenseCommand command, CancellationToken cancellationToken)
    {
        var tiersExists = await _context.TiersDepenseFonctionnement.AnyAsync(t => t.Id == command.TiersDepenseFonctionnementId, cancellationToken);
        if (!tiersExists)
        {
            _logger.LogWarning("TiersDepenseFonctionnement not found: Id {Id}", command.TiersDepenseFonctionnementId);
            return Result.Fail(EntityNotFound.Error());
        }

        var accountingYear = command.AccountingYearId.HasValue
            ? await _context.AccountingYear.FirstOrDefaultAsync(ay => ay.Id == command.AccountingYearId.Value, cancellationToken)
            : await _context.AccountingYear.FirstOrDefaultAsync(ay => ay.IsActive, cancellationToken);

        if (accountingYear == null)
        {
            _logger.LogError("No active accounting year found");
            return Result.Fail("no_active_accounting_year");
        }

        if (!Enum.TryParse<MethodePaiement>(command.MethodePaiement, ignoreCase: true, out var methodePaiement))
        {
            _logger.LogWarning("Invalid MethodePaiement: {Value}", command.MethodePaiement);
            return Result.Fail("invalid_methode_paiement");
        }

        string? documentStoragePath = null;
        if (!string.IsNullOrWhiteSpace(command.DocumentBase64))
        {
            try
            {
                var documentBytes = Convert.FromBase64String(command.DocumentBase64);
                var fileName = $"paiement_tiers_depense_{command.NumeroTransactionBancaire ?? "n/a"}_{DateTime.UtcNow:yyyyMMddHHmmss}";
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

        var entity = Domain.Entites.PaiementTiersDepense.Create(
            command.NumeroTransactionBancaire,
            command.TiersDepenseFonctionnementId,
            accountingYear.Id,
            command.Montant,
            command.DatePaiement,
            methodePaiement,
            null,
            command.NumeroChequeTraite,
            command.BanqueId,
            command.DateEcheance,
            command.Commentaire,
            command.RibCodeEtab,
            command.RibCodeAgence,
            command.RibNumeroCompte,
            command.RibCle,
            documentStoragePath,
            command.Mois);

        _context.PaiementTiersDepense.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        if (command.FactureDepenseIds is { Count: > 0 })
        {
            foreach (var factureDepenseId in command.FactureDepenseIds)
            {
                var junction = PaiementTiersDepenseFactureDepense.Create(entity.Id, factureDepenseId);
                _context.PaiementTiersDepenseFactureDepense.Add(junction);
            }
            await _context.SaveChangesAsync(cancellationToken);
        }

        _logger.LogInformation("PaiementTiersDepense created with Id {Id}", entity.Id);
        return Result.Ok(entity.Id);
    }
}
