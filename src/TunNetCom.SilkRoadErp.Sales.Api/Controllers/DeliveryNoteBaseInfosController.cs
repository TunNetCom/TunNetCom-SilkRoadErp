using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using TunNetCom.SilkRoadErp.Sales.Contracts.DeliveryNote.Responses;

namespace TunNetCom.SilkRoadErp.Sales.Api.Controllers;

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
    public IActionResult Get(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] int? customerId = null)
    {
        try
        {
            _logger.LogInformation("DeliveryNoteBaseInfosController.Get called with startDate: {StartDate}, endDate: {EndDate}, customerId: {CustomerId}", 
                startDate, endDate, customerId);

            // Build base query with filters before projection
            var baseQuery = _context.BonDeLivraison.AsNoTracking();

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

            // Now project to DTO with join
            var deliveryNoteQuery = (from bdl in baseQuery
                                    join c in _context.Client on bdl.ClientId equals c.Id into clientGroup
                                    from c in clientGroup.DefaultIfEmpty()
                                    select new GetDeliveryNoteBaseInfos
                                    {
                                        Number = bdl.Num,
                                        Date = new DateTimeOffset(bdl.Date, TimeSpan.Zero),
                                        NetAmount = bdl.NetPayer,
                                        CustomerName = c != null ? c.Nom : null,
                                        GrossAmount = bdl.TotHTva,
                                        VatAmount = bdl.TotTva,
                                        NumFacture = bdl.NumFacture,
                                        CustomerId = bdl.ClientId
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

