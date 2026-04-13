using Microsoft.EntityFrameworkCore;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services.DocumentStorage;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;
using TunNetCom.SilkRoadErp.Sales.Domain.Services;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.RetenueSourceClient.UpdateRetenueSourceClient;

public class UpdateRetenueSourceClientCommandHandler(
    SalesContext _context,
    ILogger<UpdateRetenueSourceClientCommandHandler> _logger,
    IDocumentStorageService _documentStorageService)
    : IRequestHandler<UpdateRetenueSourceClientCommand, Result>
{
    public async Task<Result> Handle(UpdateRetenueSourceClientCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("UpdateRetenueSourceClientCommand called for Facture {NumFacture}", command.NumFacture);

        var retenue = await _context.RetenueSourceClient
            .FirstOrDefaultAsync(r => r.NumFacture == command.NumFacture, cancellationToken);

        if (retenue == null)
        {
            _logger.LogEntityNotFound(nameof(RetenueSourceClient), command.NumFacture);
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
                retenue.PdfStoragePath = await _documentStorageService.SaveAsync(pdfBytes, $"retenue_client_{command.NumFacture}.pdf", cancellationToken);
                _logger.LogDebug("PDF updated for RetenueSourceClient Facture {NumFacture}", command.NumFacture);
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

        _logger.LogInformation("RetenueSourceClient updated successfully for Facture {NumFacture}", command.NumFacture);
        return Result.Ok();
    }
}

