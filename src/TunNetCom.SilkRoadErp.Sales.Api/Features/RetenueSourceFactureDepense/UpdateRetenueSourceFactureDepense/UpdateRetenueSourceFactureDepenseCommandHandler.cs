using Microsoft.EntityFrameworkCore;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services.DocumentStorage;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;
using TunNetCom.SilkRoadErp.Sales.Domain.Services;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.RetenueSourceFactureDepense.UpdateRetenueSourceFactureDepense;

public class UpdateRetenueSourceFactureDepenseCommandHandler(
    SalesContext _context,
    ILogger<UpdateRetenueSourceFactureDepenseCommandHandler> _logger,
    IDocumentStorageService _documentStorageService)
    : IRequestHandler<UpdateRetenueSourceFactureDepenseCommand, Result>
{
    public async Task<Result> Handle(UpdateRetenueSourceFactureDepenseCommand command, CancellationToken cancellationToken)
    {
        var factureAccountingYearId = await _context.FactureDepense
            .Where(f => f.Id == command.FactureDepenseId)
            .Select(f => f.AccountingYearId)
            .FirstOrDefaultAsync(cancellationToken);

        var retenue = await _context.RetenueSourceFactureDepense
            .FirstOrDefaultAsync(r => r.FactureDepenseId == command.FactureDepenseId && r.AccountingYearId == factureAccountingYearId, cancellationToken);

        if (retenue == null)
        {
            return Result.Fail(EntityNotFound.Error());
        }

        retenue.NumTej = command.NumTej;

        if (!string.IsNullOrWhiteSpace(command.PdfContent))
        {
            try
            {
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

                var pdfBytes = Convert.FromBase64String(command.PdfContent);
                retenue.PdfStoragePath = await _documentStorageService.SaveAsync(pdfBytes, $"retenue_facture_depense_{command.FactureDepenseId}.pdf", cancellationToken);
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

        _logger.LogInformation("RetenueSourceFactureDepense updated for FactureDepense Id {FactureDepenseId}", command.FactureDepenseId);
        return Result.Ok();
    }
}
