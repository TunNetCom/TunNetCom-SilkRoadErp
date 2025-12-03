using Microsoft.EntityFrameworkCore;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services.DocumentStorage;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;
using TunNetCom.SilkRoadErp.Sales.Domain.Services;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.RetenueSourceFournisseur.GetRetenueSourceFournisseurPdf;

public class GetRetenueSourceFournisseurPdfQueryHandler(
    SalesContext _context,
    ILogger<GetRetenueSourceFournisseurPdfQueryHandler> _logger,
    IDocumentStorageService _documentStorageService)
    : IRequestHandler<GetRetenueSourceFournisseurPdfQuery, Result<byte[]>>
{
    public async Task<Result<byte[]>> Handle(GetRetenueSourceFournisseurPdfQuery query, CancellationToken cancellationToken)
    {
        _logger.LogInformation("GetRetenueSourceFournisseurPdfQuery called for FactureFournisseur {NumFactureFournisseur}", query.NumFactureFournisseur);

        var retenue = await _context.RetenueSourceFournisseur
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.NumFactureFournisseur == query.NumFactureFournisseur, cancellationToken);

        if (retenue == null)
        {
            _logger.LogEntityNotFound(nameof(RetenueSourceFournisseur), query.NumFactureFournisseur);
            return Result.Fail(EntityNotFound.Error());
        }

        if (string.IsNullOrWhiteSpace(retenue.PdfStoragePath))
        {
            _logger.LogWarning("No PDF found for RetenueSourceFournisseur FactureFournisseur {NumFactureFournisseur}", query.NumFactureFournisseur);
            return Result.Fail("pdf_not_found");
        }

        try
        {
            var pdfBytes = await _documentStorageService.GetAsync(retenue.PdfStoragePath, cancellationToken);
            return Result.Ok(pdfBytes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving PDF for RetenueSourceFournisseur FactureFournisseur {NumFactureFournisseur}", query.NumFactureFournisseur);
            return Result.Fail("error_retrieving_pdf");
        }
    }
}

