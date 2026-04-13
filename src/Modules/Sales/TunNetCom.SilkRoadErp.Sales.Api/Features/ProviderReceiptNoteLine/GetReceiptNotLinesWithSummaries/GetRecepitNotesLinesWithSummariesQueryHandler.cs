using TunNetCom.SilkRoadErp.Sales.Contracts.ReceiptNoteLine.Classes;
using TunNetCom.SilkRoadErp.Sales.Contracts.ReceiptNoteLine.Response;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ProviderReceiptNoteLine.GetReceiptNotLinesWithSummaries;

internal class GetRecepitNotesLinesWithSummariesQueryHandler(
    SalesContext salesContext,
    ILogger<GetRecepitNotesLinesWithSummariesQueryHandler> _logger)
    : IRequestHandler<GetReceiptNotesLinesWithSummariesQuery, Result<GetReceiptNoteLinesByReceiptNoteIdResponse>>
{
    public async Task<Result<GetReceiptNoteLinesByReceiptNoteIdResponse>> Handle(
        GetReceiptNotesLinesWithSummariesQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var query = (from rnl in salesContext.LigneBonReception
                         where rnl.BonDeReceptionId == request.IdReceiptNote
                         select new ReceiptLine
                         { 
                             Discount = rnl.Remise,
                             ItemQuantity = rnl.QteLi,
                             ItemDescription = rnl.DesignationLi,
                             ProductReference = rnl.RefProduit,
                             LineId = rnl.IdLigne,
                             TotalExcludingTax = rnl.TotHt,
                             TotalIncludingTax = rnl.TotTtc,
                             UnitPriceExcludingTax = rnl.PrixHt,
                             VatRate = rnl.Tva
                         }).AsNoTracking().AsQueryable();

            var result = await PagedList<ReceiptLine>.ToPagedListAsync(query,request.queryStringParameters.PageNumber,request.queryStringParameters.PageSize,cancellationToken);

            var response = new GetReceiptNoteLinesByReceiptNoteIdResponse
            {
                ReceiptLinesBaseInfos = result,
                TotalNetAmount = await query.SumAsync(r => r.TotalIncludingTax, cancellationToken),
                TotalVatAmount = await query.SumAsync(r => r.TotalIncludingTax - r.TotalExcludingTax, cancellationToken),
                TotalGrossAmount = await query.SumAsync(r => r.TotalExcludingTax, cancellationToken)
            };

            return Result.Ok(response);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex,"error_fetching_receipt_note_content");
            return Result.Fail("error_fetching_receipt_note_content");
        }
    }
}
