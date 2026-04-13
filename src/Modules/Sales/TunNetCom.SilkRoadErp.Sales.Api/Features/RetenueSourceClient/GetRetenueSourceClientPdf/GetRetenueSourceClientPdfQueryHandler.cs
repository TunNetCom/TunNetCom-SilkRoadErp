using Microsoft.EntityFrameworkCore;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services.DocumentStorage;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;
using TunNetCom.SilkRoadErp.Sales.Domain.Services;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.RetenueSourceClient.GetRetenueSourceClientPdf;

public class GetRetenueSourceClientPdfQueryHandler(
    SalesContext _context,
    ILogger<GetRetenueSourceClientPdfQueryHandler> _logger,
    IDocumentStorageService _documentStorageService)
    : IRequestHandler<GetRetenueSourceClientPdfQuery, Result<byte[]>>
{
    public async Task<Result<byte[]>> Handle(GetRetenueSourceClientPdfQuery query, CancellationToken cancellationToken)
    {
        _logger.LogInformation("GetRetenueSourceClientPdfQuery called for Facture {NumFacture}", query.NumFacture);

        var retenue = await _context.RetenueSourceClient
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.NumFacture == query.NumFacture, cancellationToken);

        if (retenue == null)
        {
            _logger.LogEntityNotFound(nameof(RetenueSourceClient), query.NumFacture);
            return Result.Fail(EntityNotFound.Error());
        }

        if (string.IsNullOrWhiteSpace(retenue.PdfStoragePath))
        {
            _logger.LogWarning("No PDF found for RetenueSourceClient Facture {NumFacture}", query.NumFacture);
            return Result.Fail("pdf_not_found");
        }

        try
        {
            var pdfBytes = await _documentStorageService.GetAsync(retenue.PdfStoragePath, cancellationToken);
            return Result.Ok(pdfBytes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving PDF for RetenueSourceClient Facture {NumFacture}", query.NumFacture);
            return Result.Fail("error_retrieving_pdf");
        }
    }
}

