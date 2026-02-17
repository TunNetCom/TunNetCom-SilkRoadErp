using Microsoft.EntityFrameworkCore;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services.DocumentStorage;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;
using TunNetCom.SilkRoadErp.Sales.Domain.Services;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.RetenueSourceFactureDepense.GetRetenueSourceFactureDepensePdf;

public class GetRetenueSourceFactureDepensePdfQueryHandler(
    SalesContext _context,
    ILogger<GetRetenueSourceFactureDepensePdfQueryHandler> _logger,
    IDocumentStorageService _documentStorageService)
    : IRequestHandler<GetRetenueSourceFactureDepensePdfQuery, Result<byte[]>>
{
    public async Task<Result<byte[]>> Handle(GetRetenueSourceFactureDepensePdfQuery query, CancellationToken cancellationToken)
    {
        var factureAccountingYearId = await _context.FactureDepense
            .Where(f => f.Id == query.FactureDepenseId)
            .Select(f => f.AccountingYearId)
            .FirstOrDefaultAsync(cancellationToken);

        var retenue = await _context.RetenueSourceFactureDepense
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.FactureDepenseId == query.FactureDepenseId && r.AccountingYearId == factureAccountingYearId, cancellationToken);

        if (retenue == null)
        {
            return Result.Fail(EntityNotFound.Error());
        }

        if (string.IsNullOrWhiteSpace(retenue.PdfStoragePath))
        {
            return Result.Fail("pdf_not_found");
        }

        try
        {
            var pdfBytes = await _documentStorageService.GetAsync(retenue.PdfStoragePath, cancellationToken);
            return Result.Ok(pdfBytes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving PDF for RetenueSourceFactureDepense FactureDepenseId {FactureDepenseId}", query.FactureDepenseId);
            return Result.Fail("error_retrieving_pdf");
        }
    }
}
