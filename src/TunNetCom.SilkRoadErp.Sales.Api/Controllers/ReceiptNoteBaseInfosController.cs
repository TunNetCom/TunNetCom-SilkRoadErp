using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using TunNetCom.SilkRoadErp.Sales.Contracts.RecieptNotes;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Controllers;

public class ReceiptNoteBaseInfosController : ODataController
{
    private readonly SalesContext _context;
    private readonly ILogger<ReceiptNoteBaseInfosController> _logger;

    public ReceiptNoteBaseInfosController(
        SalesContext context,
        ILogger<ReceiptNoteBaseInfosController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [EnableQuery(MaxExpansionDepth = 3, MaxAnyAllExpressionDepth = 3)]
    public IActionResult Get(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] int? providerId = null)
    {
        try
        {
            _logger.LogInformation("ReceiptNoteBaseInfosController.Get called with startDate: {StartDate}, endDate: {EndDate}, providerId: {ProviderId}", 
                startDate, endDate, providerId);

            // Build base query with filters before projection
            var baseQuery = _context.BonDeReception.AsNoTracking();

            // Apply custom filters before projection (on entity properties)
            if (startDate.HasValue)
            {
                baseQuery = baseQuery.Where(br => br.Date >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                var endDateInclusive = endDate.Value.Date.AddDays(1).AddTicks(-1);
                baseQuery = baseQuery.Where(br => br.Date <= endDateInclusive);
            }

            if (providerId.HasValue)
            {
                baseQuery = baseQuery.Where(br => br.IdFournisseur == providerId.Value);
            }

            // Now project to DTO with join and group by to calculate totals
            var receiptNoteQuery = (from br in baseQuery
                                    join f in _context.Fournisseur on br.IdFournisseur equals f.Id into providerGroup
                                    from f in providerGroup.DefaultIfEmpty()
                                    join lbr in _context.LigneBonReception on br.Id equals lbr.BonDeReceptionId into linesGroup
                                    from lbr in linesGroup.DefaultIfEmpty()
                                    group new { br, f, lbr } by new
                                    {
                                        br.Num,
                                        br.Date,
                                        br.IdFournisseur,
                                        f.Nom
                                    } into g
                                    select new ReceiptNoteBaseInfo
                                    {
                                        Number = g.Key.Num,
                                        Date = new DateTimeOffset(g.Key.Date, TimeSpan.Zero),
                                        ProviderId = g.Key.IdFournisseur,
                                        ProviderName = g.Key.Nom,
                                        NetAmount = g.Where(x => x.lbr != null).Sum(x => x.lbr!.TotHt),
                                        VatAmount = g.Where(x => x.lbr != null).Sum(x => x.lbr!.TotTtc) - g.Where(x => x.lbr != null).Sum(x => x.lbr!.TotHt)
                                    })
                                    .AsQueryable();

            _logger.LogInformation("Returning query for receipt notes");
            return Ok(receiptNoteQuery);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in ReceiptNoteBaseInfosController.Get: {Message}", ex.Message);
            _logger.LogError(ex, "Stack trace: {StackTrace}", ex.StackTrace);
            return StatusCode(500, new { error = "Internal server error", message = ex.Message });
        }
    }
}

