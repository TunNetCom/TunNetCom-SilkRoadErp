using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using TunNetCom.SilkRoadErp.Sales.Contracts.Quotations;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Controllers;

[Authorize]
public class QuotationBaseInfosController : ODataController
{
    private readonly SalesContext _context;
    private readonly ILogger<QuotationBaseInfosController> _logger;

    public QuotationBaseInfosController(
        SalesContext context,
        ILogger<QuotationBaseInfosController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [EnableQuery(MaxExpansionDepth = 3, MaxAnyAllExpressionDepth = 3)]
    public async Task<IActionResult> Get(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] int? customerId = null,
        [FromQuery] List<int>? tagIds = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("QuotationBaseInfosController.Get called with startDate: {StartDate}, endDate: {EndDate}, customerId: {CustomerId}, tagIds: {TagIds}", 
                startDate, endDate, customerId, tagIds != null ? string.Join(",", tagIds) : "null");

            // Build base query with filters before projection
            var baseQuery = from d in _context.Devis.AsNoTracking()
                           join c in _context.Client on d.IdClient equals c.Id into clientGroup
                           from c in clientGroup.DefaultIfEmpty()
                           select new { d, c };

            // Apply custom filters before projection (on entity properties)
            if (startDate.HasValue)
            {
                baseQuery = baseQuery.Where(x => x.d.Date >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                var endDateInclusive = endDate.Value.Date.AddDays(1).AddTicks(-1);
                baseQuery = baseQuery.Where(x => x.d.Date <= endDateInclusive);
            }

            if (customerId.HasValue)
            {
                baseQuery = baseQuery.Where(x => x.d.IdClient == customerId.Value);
            }

            // Apply tag filter if provided (OR logic: document must have at least one of the selected tags)
            if (tagIds != null && tagIds.Any())
            {
                var quotationNumsWithTags = await _context.DocumentTag
                    .Where(dt => dt.DocumentType == "Devis" && tagIds.Contains(dt.TagId))
                    .Select(dt => dt.DocumentId)
                    .Distinct()
                    .ToListAsync(cancellationToken);
                
                baseQuery = baseQuery.Where(x => quotationNumsWithTags.Contains(x.d.Num));
            }

            // Load data to avoid SQL conversion issues with Statut (string -> enum -> int)
            var quotationsData = await baseQuery.ToListAsync(cancellationToken);

            // Now project to DTO in memory to avoid SQL conversion issues
            var quotationQuery = quotationsData
                .Select(x => new QuotationBaseInfo
                {
                    Number = x.d.Num,
                    Date = new DateTimeOffset(x.d.Date, TimeSpan.Zero),
                    CustomerId = x.d.IdClient,
                    CustomerName = x.c != null ? x.c.Nom : null,
                    TotalTtc = x.d.TotTtc,
                    Statut = (int)x.d.Statut,
                    StatutLibelle = x.d.Statut.ToString()
                })
                .AsQueryable();

            _logger.LogInformation("Returning query for quotations");
            return Ok(quotationQuery);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in QuotationBaseInfosController.Get: {Message}", ex.Message);
            _logger.LogError(ex, "Stack trace: {StackTrace}", ex.StackTrace);
            return StatusCode(500, new { error = "Internal server error", message = ex.Message });
        }
    }
}

