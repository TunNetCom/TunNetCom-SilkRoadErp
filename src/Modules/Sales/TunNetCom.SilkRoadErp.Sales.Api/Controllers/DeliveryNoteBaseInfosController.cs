using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using TunNetCom.SilkRoadErp.Sales.Contracts.DeliveryNote.Responses;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Controllers;

[Authorize]
public class DeliveryNoteBaseInfosController : ODataController
{
    private readonly SalesContext _context;
    private readonly ILogger<DeliveryNoteBaseInfosController> _logger;

    public DeliveryNoteBaseInfosController(
        SalesContext context,
        ILogger<DeliveryNoteBaseInfosController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [EnableQuery(MaxExpansionDepth = 3, MaxAnyAllExpressionDepth = 3)]
    public async Task<IActionResult> Get(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] int? customerId = null,
        [FromQuery] int? technicianId = null,
        [FromQuery] List<int>? tagIds = null)
    {
        try
        {
            _logger.LogInformation("DeliveryNoteBaseInfosController.Get called with startDate: {StartDate}, endDate: {EndDate}, customerId: {CustomerId}, technicianId: {TechnicianId}, tagIds: {TagIds}", 
                startDate, endDate, customerId, technicianId, tagIds != null ? string.Join(",", tagIds) : "null");

            // Build base query with filters before projection
            var baseQuery = _context.BonDeLivraison
                .AsNoTracking()
                .FilterByActiveAccountingYear();

            // Apply custom filters before projection (on entity properties)
            if (startDate.HasValue)
            {
                baseQuery = baseQuery.Where(bdl => bdl.Date >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                var endDateInclusive = endDate.Value.Date.AddDays(1).AddTicks(-1);
                baseQuery = baseQuery.Where(bdl => bdl.Date <= endDateInclusive);
            }

            if (customerId.HasValue)
            {
                baseQuery = baseQuery.Where(bdl => bdl.ClientId == customerId.Value);
            }

            if (technicianId.HasValue)
            {
                baseQuery = baseQuery.Where(bdl => bdl.InstallationTechnicianId == technicianId.Value);
            }

            // Apply tag filter if provided (OR logic: document must have at least one of the selected tags)
            if (tagIds != null && tagIds.Any())
            {
                baseQuery = baseQuery.Where(bdl => _context.DocumentTag
                    .Any(dt => dt.DocumentType == "BonDeLivraison" 
                        && dt.DocumentId == bdl.Num 
                        && tagIds.Contains(dt.TagId)));
            }

            // Load data first to avoid SQL conversion issues with Statut (string -> enum -> int)
            var deliveryNotes = await (from bdl in baseQuery
                                      join c in _context.Client on bdl.ClientId equals c.Id into clientGroup
                                      from c in clientGroup.DefaultIfEmpty()
                                      select new { bdl, c })
                                      .ToListAsync();

            // Map to DTO in memory to avoid SQL conversion issues
            var deliveryNoteQuery = deliveryNotes
                .Select(x => new GetDeliveryNoteBaseInfos
                {
                    Id = x.bdl.Id,
                    Number = x.bdl.Num,
                    Date = new DateTimeOffset(x.bdl.Date, TimeSpan.Zero),
                    NetAmount = x.bdl.NetPayer,
                    CustomerName = x.c != null ? x.c.Nom : null,
                    GrossAmount = x.bdl.TotHTva,
                    VatAmount = x.bdl.TotTva,
                    NumFacture = x.bdl.NumFacture,
                    CustomerId = x.bdl.ClientId,
                    Statut = (int)x.bdl.Statut,
                    StatutLibelle = x.bdl.Statut.ToString()
                })
                .AsQueryable();

            _logger.LogInformation("Returning query for delivery notes");
            return Ok(deliveryNoteQuery);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in DeliveryNoteBaseInfosController.Get: {Message}", ex.Message);
            _logger.LogError(ex, "Stack trace: {StackTrace}", ex.StackTrace);
            return StatusCode(500, new { error = "Internal server error", message = ex.Message });
        }
    }
}

