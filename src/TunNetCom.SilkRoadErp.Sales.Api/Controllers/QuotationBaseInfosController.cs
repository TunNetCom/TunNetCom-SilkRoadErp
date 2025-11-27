using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
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
    public IActionResult Get(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] int? customerId = null,
        [FromQuery] List<int>? tagIds = null)
    {
        try
        {
            _logger.LogInformation("QuotationBaseInfosController.Get called with startDate: {StartDate}, endDate: {EndDate}, customerId: {CustomerId}, tagIds: {TagIds}", 
                startDate, endDate, customerId, tagIds != null ? string.Join(",", tagIds) : "null");

            // Build base query with filters before projection
            var baseQuery = _context.Devis.AsNoTracking();

            // Apply custom filters before projection (on entity properties)
            if (startDate.HasValue)
            {
                baseQuery = baseQuery.Where(d => d.Date >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                var endDateInclusive = endDate.Value.Date.AddDays(1).AddTicks(-1);
                baseQuery = baseQuery.Where(d => d.Date <= endDateInclusive);
            }

            if (customerId.HasValue)
            {
                baseQuery = baseQuery.Where(d => d.IdClient == customerId.Value);
            }

            // Apply tag filter if provided (OR logic: document must have at least one of the selected tags)
            if (tagIds != null && tagIds.Any())
            {
                baseQuery = baseQuery.Where(d => _context.DocumentTag
                    .Any(dt => dt.DocumentType == "Devis" 
                        && dt.DocumentId == d.Num 
                        && tagIds.Contains(dt.TagId)));
            }

            // Now project to DTO with join
            var quotationQuery = (from d in baseQuery
                                  join c in _context.Client on d.IdClient equals c.Id into clientGroup
                                  from c in clientGroup.DefaultIfEmpty()
                                  select new QuotationBaseInfo
                                  {
                                      Number = d.Num,
                                      Date = new DateTimeOffset(d.Date, TimeSpan.Zero),
                                      CustomerId = d.IdClient,
                                      CustomerName = c != null ? c.Nom : null,
                                      TotalTtc = d.TotTtc
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

