using TunNetCom.SilkRoadErp.Sales.Api.Features.AppParameters.GetAppParameters;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services.DocumentStorage;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;
using TunNetCom.SilkRoadErp.Sales.Domain.Services;
using Microsoft.EntityFrameworkCore;
using RetenueSourceFournisseurEntity = TunNetCom.SilkRoadErp.Sales.Domain.Entites.RetenueSourceFournisseur;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.RetenueSourceFournisseur.CreateRetenueSourceFournisseur;

public class CreateRetenueSourceFournisseurCommandHandler(
    SalesContext _context,
    ILogger<CreateRetenueSourceFournisseurCommandHandler> _logger,
    IMediator _mediator,
    IDocumentStorageService _documentStorageService,
    IActiveAccountingYearService _activeAccountingYearService,
    IAccountingYearFinancialParametersService _financialParametersService)
    : IRequestHandler<CreateRetenueSourceFournisseurCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateRetenueSourceFournisseurCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("CreateRetenueSourceFournisseurCommand called for FactureFournisseur {NumFactureFournisseur}", command.NumFactureFournisseur);

        // Get app parameters
        var appParamsResult = await _mediator.Send(new GetAppParametersQuery(), cancellationToken);
        if (appParamsResult.IsFailed)
        {
            return Result.Fail("failed_to_retrieve_app_parameters");
        }
        var appParams = appParamsResult.Value;

        // Validate facture fournisseur exists
        var factureFournisseur = await _context.FactureFournisseur
            .Include(f => f.BonDeReception)
                .ThenInclude(br => br.LigneBonReception)
            .Include(f => f.IdFournisseurNavigation)
            .FirstOrDefaultAsync(f => f.Num == command.NumFactureFournisseur, cancellationToken);

        if (factureFournisseur == null)
        {
            _logger.LogEntityNotFound(nameof(FactureFournisseur), command.NumFactureFournisseur);
            return Result.Fail(EntityNotFound.Error());
        }

        if (factureFournisseur.IdFournisseurNavigation?.ExonereRetenueSource == true)
        {
            _logger.LogWarning("Cannot create RetenueSourceFournisseur: provider {ProviderId} is exempt from withholding tax", factureFournisseur.IdFournisseur);
            return Result.Fail("fournisseur_exonere_retenue_source");
        }

        // Check if retenue already exists
        var retenueExists = await _context.RetenueSourceFournisseur
            .AnyAsync(r => r.NumFactureFournisseur == command.NumFactureFournisseur, cancellationToken);

        if (retenueExists)
        {
            _logger.LogWarning("RetenueSourceFournisseur already exists for FactureFournisseur {NumFactureFournisseur}", command.NumFactureFournisseur);
            return Result.Fail("retenue_already_exists");
        }

        // Calculate montant TTC (somme des BonDeReception.LigneBonReception.TotTtc)
        var montantTTC = factureFournisseur.BonDeReception
            .SelectMany(br => br.LigneBonReception)
            .Sum(l => l.TotTtc);

        // Soustraire les avoirs financiers et les factures avoir fournisseur (avoirs normaux) rattachés à cette facture
        var totalAvoirsFinanciers = await _context.AvoirFinancierFournisseurs
            .Where(a => a.NumFactureFournisseur == command.NumFactureFournisseur)
            .SumAsync(a => a.TotTtc, cancellationToken);
        var totalFacturesAvoir = await _context.FactureAvoirFournisseur
            .Where(fa => fa.FactureFournisseurId == factureFournisseur.Id)
            .SelectMany(fa => fa.AvoirFournisseur)
            .SelectMany(a => a.LigneAvoirFournisseur)
            .SumAsync(l => l.TotTtc, cancellationToken);
        var montantTTCApresAvoirs = montantTTC - totalAvoirsFinanciers - totalFacturesAvoir;

        // Get active accounting year ID
        var activeAccountingYearId = await _activeAccountingYearService.GetActiveAccountingYearIdAsync(cancellationToken);
        if (!activeAccountingYearId.HasValue)
        {
            _logger.LogError("No active accounting year found");
            return Result.Fail("no_active_accounting_year");
        }

        // Get timbre from financial parameters service
        var timbre = await _financialParametersService.GetTimbreAsync(appParams.Timbre, cancellationToken);

        // Montant avant retenue = facture TTC - avoirs financiers + timbre
        var montantTTCAvecTimbre = montantTTCApresAvoirs + timbre;

        // Get seuil from financial parameters service (migrated to AccountingYear)
        var seuilRetenueSource = await _financialParametersService.GetSeuilRetenueSourceAsync(1000, cancellationToken);

        // Validate threshold (montant après déduction des avoirs financiers + timbre)
        if (montantTTCAvecTimbre < seuilRetenueSource)
        {
            _logger.LogWarning("Montant TTC après avoirs {MontantTTC} is below threshold {Seuil} for FactureFournisseur {NumFactureFournisseur}",
                montantTTCAvecTimbre, seuilRetenueSource, command.NumFactureFournisseur);
            return Result.Fail($"seuil_non_atteint: Le montant TTC ({montantTTCAvecTimbre}) doit être supérieur ou égal au seuil ({seuilRetenueSource})");
        }

        // Base pour le calcul de la retenue = montant TTC après avoirs (sans timbre)
        var montantTTCHorsTimbre = montantTTCApresAvoirs;

        // Get taux retenu: use supplier's rate if defined, otherwise use accounting year rate
        var pourcentageRetenu = await _financialParametersService.GetPourcentageRetenuAsync(appParams.PourcentageRetenu, cancellationToken);
        var tauxRetenu = factureFournisseur.IdFournisseurNavigation.TauxRetenu ?? pourcentageRetenu;
        _logger.LogInformation("Using taux retenu {TauxRetenu} for FactureFournisseur {NumFactureFournisseur} (from supplier: {FromSupplier})",
            tauxRetenu, command.NumFactureFournisseur, factureFournisseur.IdFournisseurNavigation.TauxRetenu.HasValue ? "Yes" : "No (app params)");

        // Calculate montant après retenue (on amount after avoirs, without timbre)
        var montantApresRetenuSansTimbre = montantTTCHorsTimbre * (1 - (decimal)tauxRetenu / 100);

        // Add timbre after retenue calculation
        var montantApresRetenuAvecTimbre = montantApresRetenuSansTimbre + timbre;

        // Store PDF if provided
        string? pdfStoragePath = null;
        if (!string.IsNullOrWhiteSpace(command.PdfContent))
        {
            try
            {
                var pdfBytes = Convert.FromBase64String(command.PdfContent);
                pdfStoragePath = await _documentStorageService.SaveAsync(pdfBytes, $"retenue_fournisseur_{command.NumFactureFournisseur}.pdf", cancellationToken);
                _logger.LogDebug("PDF stored for RetenueSourceFournisseur FactureFournisseur {NumFactureFournisseur}", command.NumFactureFournisseur);
            }
            catch (FormatException ex)
            {
                _logger.LogError(ex, "Invalid Base64 format for PDF content");
                return Result.Fail("invalid_pdf_format");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error storing PDF");
                return Result.Fail("error_storing_pdf");
            }
        }

        // Create retenue
        var retenue = new RetenueSourceFournisseurEntity
        {
            NumFactureFournisseur = command.NumFactureFournisseur,
            NumTej = command.NumTej,
            MontantAvantRetenu = montantTTCAvecTimbre, // Montant TTC - avoirs financiers + timbre
            TauxRetenu = tauxRetenu,
            MontantApresRetenu = montantApresRetenuAvecTimbre,
            PdfStoragePath = pdfStoragePath,
            DateCreation = DateTime.UtcNow,
            AccountingYearId = activeAccountingYearId.Value
        };

        _context.RetenueSourceFournisseur.Add(retenue);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("RetenueSourceFournisseur created successfully with Id {Id} for FactureFournisseur {NumFactureFournisseur}",
            retenue.Id, command.NumFactureFournisseur);

        return Result.Ok(retenue.Id);
    }
}

