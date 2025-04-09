using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ReceiptNote.GetReceiptNoteWithDetails.Calculation;

public class ReceiptNoteSumCalculator(
    SalesContext _context)
{

    //public Task<ReceiptNoteDetailsResponse> CalculateTotalHTvaAsync(
    //   ReceiptNoteResponse receiptNote,
    //   CancellationToken cancellationToken)
    //{
    //    var invoices = _context.LigneBonReception
    //          .Where(f => f.NumBonRec == receiptNote.Num)
    //          .Select(f => new LigneBonReception
    //          {
    //              IdLigne = f.NumBonRec,
    //              TotHt = f.TotHt,
    //              Tva = f.Tva,
    //              TotTtc = f.TotTtc,
    //          })
    //          .ToListAsync(cancellationToken);
    //    var recipt = new ReceiptNoteDetailsResponse
    //    {
    //        TotHTva = invoices.Result.Sum(d => d.TotHt),
    //        TotTva = invoices.Result.Sum(d => d.TotHt),
    //        TotTTC = invoices.Result.Sum(d => d.TotTtc)
    //    };
    //    return recipt;
    //}
}
