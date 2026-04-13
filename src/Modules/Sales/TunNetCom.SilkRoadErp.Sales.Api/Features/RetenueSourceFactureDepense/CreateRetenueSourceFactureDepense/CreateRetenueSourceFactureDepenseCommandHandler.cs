using Microsoft.EntityFrameworkCore;
using TunNetCom.SilkRoadErp.Sales.Api.Features.AppParameters.GetAppParameters;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services.DocumentStorage;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;
using TunNetCom.SilkRoadErp.Sales.Domain.Services;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.RetenueSourceFactureDepense.CreateRetenueSourceFactureDepense;

public class CreateRetenueSourceFactureDepenseCommandHandler(
    SalesContext _context,
    ILogger<CreateRetenueSourceFactureDepenseCommandHandler> _logger,
    IMediator _mediator,
    IDocumentStorageService _documentStorageService,
    IActiveAccountingYearService _activeAccountingYearService,
    IAccountingYearFinancialParametersService _financialParametersService)
    : IRequestHandler<CreateRetenueSourceFactureDepenseCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateRetenueSourceFactureDepenseCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("CreateRetenueSourceFactureDepenseCommand called for FactureDepense Id {FactureDepenseId}", command.FactureDepenseId);

        var factureDepense = await _context.FactureDepense
            .Include(f => f.IdTiersDepenseFonctionnementNavigation)
            .FirstOrDefaultAsync(f => f.Id == command.FactureDepenseId, cancellationToken);

        if (factureDepense == null)
        {
            _logger.LogEntityNotFound(nameof(FactureDepense), command.FactureDepenseId);
            return Result.Fail(EntityNotFound.Error());
        }

        if (factureDepense.IdTiersDepenseFonctionnementNavigation?.ExonereRetenueSource == true)
        {
            _logger.LogWarning("Cannot create RetenueSourceFactureDepense: tiers is exempt from withholding tax for FactureDepense Id {Id}", command.FactureDepenseId);
            return Result.Fail("tiers_exonere_retenue_source");
        }

        var accountingYearId = factureDepense.AccountingYearId;

        var retenueExists = await _context.RetenueSourceFactureDepense
            .AnyAsync(r => r.FactureDepenseId == command.FactureDepenseId && r.AccountingYearId == accountingYearId, cancellationToken);

        if (retenueExists)
        {
            _logger.LogWarning("RetenueSourceFactureDepense already exists for FactureDepense Id {FactureDepenseId}", command.FactureDepenseId);
            return Result.Fail("retenue_already_exists");
        }

        var montantTTC = factureDepense.MontantTotal;

        var activeAccountingYearId = await _activeAccountingYearService.GetActiveAccountingYearIdAsync(cancellationToken);
        if (!activeAccountingYearId.HasValue)
        {
            _logger.LogError("No active accounting year found");
            return Result.Fail("no_active_accounting_year");
        }

        var seuilRetenueSource = await _financialParametersService.GetSeuilRetenueSourceAsync(1000, cancellationToken);
        if (montantTTC < seuilRetenueSource)
        {
            _logger.LogWarning("Montant TTC {MontantTTC} is below threshold {Seuil} for FactureDepense Id {FactureDepenseId}",
                montantTTC, seuilRetenueSource, command.FactureDepenseId);
            return Result.Fail($"seuil_non_atteint: Le montant TTC ({montantTTC}) doit être supérieur ou égal au seuil ({seuilRetenueSource})");
        }

        var appParamsResult = await _mediator.Send(new GetAppParametersQuery(), cancellationToken);
        var pourcentageRetenu = await _financialParametersService.GetPourcentageRetenuAsync(
            appParamsResult.IsSuccess ? appParamsResult.Value.PourcentageRetenu : 10,
            cancellationToken);
        var montantApresRetenu = montantTTC * (1 - (decimal)pourcentageRetenu / 100);

        string? pdfStoragePath = null;
        if (!string.IsNullOrWhiteSpace(command.PdfContent))
        {
            try
            {
                var pdfBytes = Convert.FromBase64String(command.PdfContent);
                pdfStoragePath = await _documentStorageService.SaveAsync(pdfBytes, $"retenue_facture_depense_{command.FactureDepenseId}.pdf", cancellationToken);
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

        var retenue = new TunNetCom.SilkRoadErp.Sales.Domain.Entites.RetenueSourceFactureDepense
        {
            FactureDepenseId = command.FactureDepenseId,
            NumTej = command.NumTej,
            MontantAvantRetenu = montantTTC,
            TauxRetenu = pourcentageRetenu,
            MontantApresRetenu = montantApresRetenu,
            PdfStoragePath = pdfStoragePath,
            DateCreation = DateTime.UtcNow,
            AccountingYearId = accountingYearId
        };

        _context.RetenueSourceFactureDepense.Add(retenue);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("RetenueSourceFactureDepense created with Id {Id} for FactureDepense Id {FactureDepenseId}",
            retenue.Id, command.FactureDepenseId);

        return Result.Ok(retenue.Id);
    }
}
