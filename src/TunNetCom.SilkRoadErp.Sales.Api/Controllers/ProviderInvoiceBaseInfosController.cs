using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using TunNetCom.SilkRoadErp.Sales.Contracts.ProviderInvoice;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Controllers;

[Authorize]
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
    public async Task<IActionResult> Get(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] int? providerId = null,
        [FromQuery] List<int>? tagIds = null,
        CancellationToken cancellationToken = default)
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

            // Build query with joins before loading to memory
            var queryWithJoins = from ff in baseQuery
                                join br in _context.BonDeReception on ff.Num equals br.NumFactureFournisseur into receiptNotesGroup
                                from br in receiptNotesGroup.DefaultIfEmpty()
                                join lbr in _context.LigneBonReception on br.Id equals lbr.BonDeReceptionId into linesGroup
                                from lbr in linesGroup.DefaultIfEmpty()
                                select new { ff, lbr };

            // Load data to avoid SQL conversion issues with Statut (string -> enum -> int)
            var invoicesData = await queryWithJoins.ToListAsync(cancellationToken);

            // Group and calculate in memory to avoid SQL conversion issues
            var providerInvoiceQuery = invoicesData
                .GroupBy(x => new { x.ff.Num, x.ff.Date, x.ff.IdFournisseur, x.ff.Statut })
                .Select(g => new ProviderInvoiceBaseInfo
                {
                    Number = g.Key.Num,
                    Date = new DateTimeOffset(g.Key.Date, TimeSpan.Zero),
                    ProviderId = g.Key.IdFournisseur,
                    NetAmount = g.Where(x => x.lbr != null).Sum(x => x.lbr!.TotHt),
                    VatAmount = g.Where(x => x.lbr != null).Sum(x => x.lbr!.TotTtc) - g.Where(x => x.lbr != null).Sum(x => x.lbr!.TotHt),
                    Statut = (int)g.Key.Statut,
                    StatutLibelle = g.Key.Statut.ToString()
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

