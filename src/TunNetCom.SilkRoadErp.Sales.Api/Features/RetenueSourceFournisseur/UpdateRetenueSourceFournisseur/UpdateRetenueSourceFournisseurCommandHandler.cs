using Microsoft.EntityFrameworkCore;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services.DocumentStorage;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;
using TunNetCom.SilkRoadErp.Sales.Domain.Services;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.RetenueSourceFournisseur.UpdateRetenueSourceFournisseur;

public class UpdateRetenueSourceFournisseurCommandHandler(
    SalesContext _context,
    ILogger<UpdateRetenueSourceFournisseurCommandHandler> _logger,
    IDocumentStorageService _documentStorageService)
    : IRequestHandler<UpdateRetenueSourceFournisseurCommand, Result>
{
    public async Task<Result> Handle(UpdateRetenueSourceFournisseurCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("UpdateRetenueSourceFournisseurCommand called for FactureFournisseur {NumFactureFournisseur}", command.NumFactureFournisseur);

        var retenue = await _context.RetenueSourceFournisseur
            .FirstOrDefaultAsync(r => r.NumFactureFournisseur == command.NumFactureFournisseur, cancellationToken);

        if (retenue == null)
        {
            _logger.LogEntityNotFound(nameof(RetenueSourceFournisseur), command.NumFactureFournisseur);
            return Result.Fail(EntityNotFound.Error());
        }

        // Update NumTej
        retenue.NumTej = command.NumTej;

        // Update PDF if provided
        if (!string.IsNullOrWhiteSpace(command.PdfContent))
        {
            try
            {
                // Delete old PDF if exists
                if (!string.IsNullOrWhiteSpace(retenue.PdfStoragePath))
                {
                    try
                    {
                        await _documentStorageService.DeleteAsync(retenue.PdfStoragePath, cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Error deleting old PDF, continuing with new upload");
                    }
                }

                // Store new PDF
                var pdfBytes = Convert.FromBase64String(command.PdfContent);
                retenue.PdfStoragePath = await _documentStorageService.SaveAsync(pdfBytes, $"retenue_fournisseur_{command.NumFactureFournisseur}.pdf", cancellationToken);
                _logger.LogDebug("PDF updated for RetenueSourceFournisseur FactureFournisseur {NumFactureFournisseur}", command.NumFactureFournisseur);
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

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("RetenueSourceFournisseur updated successfully for FactureFournisseur {NumFactureFournisseur}", command.NumFactureFournisseur);
        return Result.Ok();
    }
}

