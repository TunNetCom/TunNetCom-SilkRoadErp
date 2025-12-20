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
        _logger.LogInformation("CreatePaiementClientCommand called with ClientId {ClientId} and Numero {Numero}", command.ClientId, command.Numero);

        // Validate client exists
        var clientExists = await _context.Client.AnyAsync(c => c.Id == command.ClientId, cancellationToken);
        if (!clientExists)
        {
            return Result.Fail("client_not_found");
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
        var numeroExists = await _context.PaiementClient
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
        string? documentStoragePath = null;
        if (!string.IsNullOrWhiteSpace(command.DocumentBase64))
        {
            try
            {
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

        var paiement = Domain.Entites.PaiementClient.CreatePaiementClient(
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

        _context.PaiementClient.Add(paiement);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("PaiementClient created successfully with Id {Id}", paiement.Id);
        return Result.Ok(paiement.Id);
    }
}

