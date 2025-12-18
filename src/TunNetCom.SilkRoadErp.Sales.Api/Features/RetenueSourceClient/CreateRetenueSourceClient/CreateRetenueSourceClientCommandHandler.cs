using Microsoft.EntityFrameworkCore;
using TunNetCom.SilkRoadErp.Sales.Api.Features.AppParameters.GetAppParameters;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services.DocumentStorage;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;
using TunNetCom.SilkRoadErp.Sales.Domain.Services;
using RetenueSourceClientEntity = TunNetCom.SilkRoadErp.Sales.Domain.Entites.RetenueSourceClient;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.RetenueSourceClient.CreateRetenueSourceClient;

public class CreateRetenueSourceClientCommandHandler(
    SalesContext _context,
    ILogger<CreateRetenueSourceClientCommandHandler> _logger,
    IMediator _mediator,
    IDocumentStorageService _documentStorageService)
    : IRequestHandler<CreateRetenueSourceClientCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateRetenueSourceClientCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("CreateRetenueSourceClientCommand called for Facture {NumFacture}", command.NumFacture);

        // Get app parameters
        var appParamsResult = await _mediator.Send(new GetAppParametersQuery(), cancellationToken);
        if (appParamsResult.IsFailed)
        {
            return Result.Fail("failed_to_retrieve_app_parameters");
        }
        var appParams = appParamsResult.Value;

        // Validate facture exists
        var facture = await _context.Facture
            .Include(f => f.BonDeLivraison)
            .FirstOrDefaultAsync(f => f.Num == command.NumFacture, cancellationToken);

        if (facture == null)
        {
            _logger.LogEntityNotFound(nameof(Facture), command.NumFacture);
            return Result.Fail(EntityNotFound.Error());
        }

        // Check if retenue already exists
        var retenueExists = await _context.RetenueSourceClient
            .AnyAsync(r => r.NumFacture == command.NumFacture, cancellationToken);

        if (retenueExists)
        {
            _logger.LogWarning("RetenueSourceClient already exists for Facture {NumFacture}", command.NumFacture);
            return Result.Fail("retenue_already_exists");
        }

        // Calculate montant TTC with timbre for threshold validation
        var montantTTCAvecTimbre = facture.BonDeLivraison.Sum(b => b.NetPayer) + appParams.Timbre;

        // Validate threshold (on the full TTC amount including timbre)
        if (montantTTCAvecTimbre < appParams.SeuilRetenueSource)
        {
            _logger.LogWarning("Montant TTC {MontantTTC} is below threshold {Seuil} for Facture {NumFacture}",
                montantTTCAvecTimbre, appParams.SeuilRetenueSource, command.NumFacture);
            return Result.Fail($"seuil_non_atteint: Le montant TTC ({montantTTCAvecTimbre}) doit être supérieur ou égal au seuil ({appParams.SeuilRetenueSource})");
        }

        // Get active accounting year
        var activeAccountingYear = await _context.AccountingYear
            .FirstOrDefaultAsync(ay => ay.IsActive, cancellationToken);

        if (activeAccountingYear == null)
        {
            _logger.LogError("No active accounting year found");
            return Result.Fail("no_active_accounting_year");
        }

        // Calculate montant TTC without timbre (retenue is calculated on amount without timbre)
        var montantTTCHorsTimbre = facture.BonDeLivraison.Sum(b => b.NetPayer);

        // Calculate montant après retenue (on amount without timbre)
        var tauxRetenu = appParams.PourcentageRetenu;
        var montantApresRetenuSansTimbre = montantTTCHorsTimbre * (1 - (decimal)tauxRetenu / 100);

        // Add timbre after retenue calculation
        var montantApresRetenuAvecTimbre = montantApresRetenuSansTimbre + appParams.Timbre;

        // Store PDF if provided
        string? pdfStoragePath = null;
        if (!string.IsNullOrWhiteSpace(command.PdfContent))
        {
            try
            {
                var pdfBytes = Convert.FromBase64String(command.PdfContent);
                pdfStoragePath = await _documentStorageService.SaveAsync(pdfBytes, $"retenue_client_{command.NumFacture}.pdf", cancellationToken);
                _logger.LogDebug("PDF stored for RetenueSourceClient Facture {NumFacture}", command.NumFacture);
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
        var retenue = new RetenueSourceClientEntity
        {
            NumFacture = command.NumFacture,
            NumTej = command.NumTej,
            MontantAvantRetenu = montantTTCHorsTimbre,
            TauxRetenu = tauxRetenu,
            MontantApresRetenu = montantApresRetenuAvecTimbre,
            PdfStoragePath = pdfStoragePath,
            DateCreation = DateTime.UtcNow,
            AccountingYearId = activeAccountingYear.Id
        };

        _context.RetenueSourceClient.Add(retenue);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("RetenueSourceClient created successfully with Id {Id} for Facture {NumFacture}",
            retenue.Id, command.NumFacture);

        return Result.Ok(retenue.Id);
    }
}

