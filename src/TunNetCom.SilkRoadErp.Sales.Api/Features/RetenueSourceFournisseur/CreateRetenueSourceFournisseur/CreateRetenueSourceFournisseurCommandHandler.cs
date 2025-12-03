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
    IDocumentStorageService _documentStorageService)
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
            .FirstOrDefaultAsync(f => f.Num == command.NumFactureFournisseur, cancellationToken);

        if (factureFournisseur == null)
        {
            _logger.LogEntityNotFound(nameof(FactureFournisseur), command.NumFactureFournisseur);
            return Result.Fail(EntityNotFound.Error());
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

        // Validate threshold
        if (montantTTC < appParams.SeuilRetenueSource)
        {
            _logger.LogWarning("Montant TTC {MontantTTC} is below threshold {Seuil} for FactureFournisseur {NumFactureFournisseur}",
                montantTTC, appParams.SeuilRetenueSource, command.NumFactureFournisseur);
            return Result.Fail($"seuil_non_atteint: Le montant TTC ({montantTTC}) doit être supérieur ou égal au seuil ({appParams.SeuilRetenueSource})");
        }

        // Get active accounting year
        var activeAccountingYear = await _context.AccountingYear
            .FirstOrDefaultAsync(ay => ay.IsActive, cancellationToken);

        if (activeAccountingYear == null)
        {
            _logger.LogError("No active accounting year found");
            return Result.Fail("no_active_accounting_year");
        }

        // Calculate montant après retenue
        var tauxRetenu = appParams.PourcentageRetenu;
        var montantApresRetenu = montantTTC * (1 - (decimal)tauxRetenu / 100);

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
            MontantAvantRetenu = montantTTC,
            TauxRetenu = tauxRetenu,
            MontantApresRetenu = montantApresRetenu,
            PdfStoragePath = pdfStoragePath,
            DateCreation = DateTime.UtcNow,
            AccountingYearId = activeAccountingYear.Id
        };

        _context.RetenueSourceFournisseur.Add(retenue);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("RetenueSourceFournisseur created successfully with Id {Id} for FactureFournisseur {NumFactureFournisseur}",
            retenue.Id, command.NumFactureFournisseur);

        return Result.Ok(retenue.Id);
    }
}

