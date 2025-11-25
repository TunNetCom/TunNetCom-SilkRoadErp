using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using TunNetCom.SilkRoadErp.Sales.Contracts.ProviderInvoice;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Controllers;

public class ProviderInvoiceBaseInfosController : ODataController
{
    private readonly SalesContext _context;
    private readonly ILogger<ProviderInvoiceBaseInfosController> _logger;

    public ProviderInvoiceBaseInfosController(
        SalesContext context,
        ILogger<ProviderInvoiceBaseInfosController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [EnableQuery(MaxExpansionDepth = 3, MaxAnyAllExpressionDepth = 3)]
    public IActionResult Get(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] int? providerId = null,
        [FromQuery] List<int>? tagIds = null)
    {
        try
        {
            _logger.LogInformation("ProviderInvoiceBaseInfosController.Get called with startDate: {StartDate}, endDate: {EndDate}, providerId: {ProviderId}, tagIds: {TagIds}", 
                startDate, endDate, providerId, tagIds != null ? string.Join(",", tagIds) : "null");

            // Build base query with filters before projection
            var baseQuery = _context.FactureFournisseur.AsNoTracking();

            // Apply custom filters before projection (on entity properties)
            if (startDate.HasValue)
            {
                baseQuery = baseQuery.Where(ff => ff.Date >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                var endDateInclusive = endDate.Value.Date.AddDays(1).AddTicks(-1);
                baseQuery = baseQuery.Where(ff => ff.Date <= endDateInclusive);
            }

            if (providerId.HasValue)
            {
                baseQuery = baseQuery.Where(ff => ff.IdFournisseur == providerId.Value);
            }

            // Apply tag filter if provided (OR logic: document must have at least one of the selected tags)
            if (tagIds != null && tagIds.Any())
            {
                baseQuery = baseQuery.Where(ff => _context.DocumentTag
                    .Any(dt => dt.DocumentType == "FactureFournisseur" 
                        && dt.DocumentId == ff.Num 
                        && tagIds.Contains(dt.TagId)));
            }

            // Now project to DTO - calculate totals from receipt notes
            var providerInvoiceQuery = (from ff in baseQuery
                                       join br in _context.BonDeReception on ff.Num equals br.NumFactureFournisseur into receiptNotesGroup
                                       from br in receiptNotesGroup.DefaultIfEmpty()
                                       join lbr in _context.LigneBonReception on br.Id equals lbr.BonDeReceptionId into linesGroup
                                       from lbr in linesGroup.DefaultIfEmpty()
                                       group new { ff, lbr } by new
                                       {
                                           ff.Num,
                                           ff.Date,
                                           ff.IdFournisseur
                                       } into g
                                       select new ProviderInvoiceBaseInfo
                                       {
                                           Number = g.Key.Num,
                                           Date = new DateTimeOffset(g.Key.Date, TimeSpan.Zero),
                                           ProviderId = g.Key.IdFournisseur,
                                           NetAmount = g.Where(x => x.lbr != null).Sum(x => x.lbr!.TotHt),
                                           VatAmount = g.Where(x => x.lbr != null).Sum(x => x.lbr!.TotTtc) - g.Where(x => x.lbr != null).Sum(x => x.lbr!.TotHt)
                                       })
                                       .AsQueryable();

            _logger.LogInformation("Returning query for provider invoices");
            return Ok(providerInvoiceQuery);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in ProviderInvoiceBaseInfosController.Get: {Message}", ex.Message);
            _logger.LogError(ex, "Stack trace: {StackTrace}", ex.StackTrace);
            return StatusCode(500, new { error = "Internal server error", message = ex.Message });
        }
    }
}

