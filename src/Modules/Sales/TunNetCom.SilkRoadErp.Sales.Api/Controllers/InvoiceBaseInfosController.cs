using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using TunNetCom.SilkRoadErp.Sales.Api.Features.AppParameters.GetAppParameters;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Controllers;

[Authorize]
public class InvoiceBaseInfosController : ODataController
{
    private readonly SalesContext _context;
    private readonly ILogger<InvoiceBaseInfosController> _logger;
    private readonly IMediator _mediator;
    private readonly IAccountingYearFinancialParametersService _financialParametersService;

    public InvoiceBaseInfosController(
        SalesContext context,
        ILogger<InvoiceBaseInfosController> logger,
        IMediator mediator,
        IAccountingYearFinancialParametersService financialParametersService)
    {
        _context = context;
        _logger = logger;
        _mediator = mediator;
        _financialParametersService = financialParametersService;
    }

    [EnableQuery(MaxExpansionDepth = 3, MaxAnyAllExpressionDepth = 3, PageSize = 50)]
    public async Task<IActionResult> Get(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] int? customerId = null,
        [FromQuery] List<int>? tagIds = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("InvoiceBaseInfosController.Get called with startDate: {StartDate} (HasValue: {HasStartDate}), endDate: {EndDate} (HasValue: {HasEndDate}), customerId: {CustomerId}, tagIds: {TagIds}", 
                startDate, startDate.HasValue, endDate, endDate.HasValue, customerId, tagIds != null ? string.Join(",", tagIds) : "null");
            
            // #region agent log
            try { System.IO.File.AppendAllText(@"d:\Workspaces\SilkRoad\TunNetCom-SilkRoadErp\.cursor\debug.log", Newtonsoft.Json.JsonConvert.SerializeObject(new { sessionId = "debug-session", runId = "run1", hypothesisId = "H2,H3", location = "InvoiceBaseInfosController.cs:40", message = "Controller entry", data = new { hasStartDate = startDate.HasValue, startDate = startDate?.ToString("o"), startDateKind = startDate?.Kind.ToString(), hasEndDate = endDate.HasValue, endDate = endDate?.ToString("o"), endDateKind = endDate?.Kind.ToString(), customerId }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
            // #endregion

            // Get timbre from financial parameters service
            var appParams = await _mediator.Send(new GetAppParametersQuery());
            var timbre = await _financialParametersService.GetTimbreAsync(appParams.Value.Timbre, cancellationToken);
            
            // Build base query with filters BEFORE loading to memory
            var baseQuery = _context.Facture
                .AsNoTracking()
                .FilterByActiveAccountingYear();
            
            // #region agent log
            var countBeforeFilters = await baseQuery.CountAsync(cancellationToken);
            try { System.IO.File.AppendAllText(@"d:\Workspaces\SilkRoad\TunNetCom-SilkRoadErp\.cursor\debug.log", Newtonsoft.Json.JsonConvert.SerializeObject(new { sessionId = "debug-session", runId = "run1", hypothesisId = "H4", location = "InvoiceBaseInfosController.cs:50", message = "After FilterByActiveAccountingYear", data = new { countBeforeFilters }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
            // #endregion

            // Apply custom filters before projection (on entity properties)
            if (startDate.HasValue)
            {
                _logger.LogInformation("Applying startDate filter: {StartDate}", startDate.Value);
                baseQuery = baseQuery.Where(f => f.Date >= startDate.Value);
                // #region agent log
                var countAfterStart = await baseQuery.CountAsync(cancellationToken);
                try { System.IO.File.AppendAllText(@"d:\Workspaces\SilkRoad\TunNetCom-SilkRoadErp\.cursor\debug.log", Newtonsoft.Json.JsonConvert.SerializeObject(new { sessionId = "debug-session", runId = "run1", hypothesisId = "H3,H5", location = "InvoiceBaseInfosController.cs:65", message = "After startDate filter", data = new { countAfterStart, startDate = startDate.Value.ToString("o"), startDateKind = startDate.Value.Kind.ToString() }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
                // #endregion
            }
            else
            {
                // #region agent log
                try { System.IO.File.AppendAllText(@"d:\Workspaces\SilkRoad\TunNetCom-SilkRoadErp\.cursor\debug.log", Newtonsoft.Json.JsonConvert.SerializeObject(new { sessionId = "debug-session", runId = "run1", hypothesisId = "H3", location = "InvoiceBaseInfosController.cs:72", message = "StartDate is NULL", data = new { }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
                // #endregion
            }

            if (endDate.HasValue)
            {
                // If endDate already has time component (not midnight), use it as-is
                // Otherwise, set to end of day
                var endDateInclusive = endDate.Value.TimeOfDay == TimeSpan.Zero 
                    ? endDate.Value.Date.AddDays(1).AddTicks(-1) 
                    : endDate.Value;
                _logger.LogInformation("Applying endDate filter: {EndDate} (inclusive: {EndDateInclusive})", endDate.Value, endDateInclusive);
                baseQuery = baseQuery.Where(f => f.Date <= endDateInclusive);
                // #region agent log
                var countAfterEnd = await baseQuery.CountAsync(cancellationToken);
                try { System.IO.File.AppendAllText(@"d:\Workspaces\SilkRoad\TunNetCom-SilkRoadErp\.cursor\debug.log", Newtonsoft.Json.JsonConvert.SerializeObject(new { sessionId = "debug-session", runId = "run1", hypothesisId = "H3,H5", location = "InvoiceBaseInfosController.cs:81", message = "After endDate filter", data = new { countAfterEnd, endDate = endDate.Value.ToString("o"), endDateInclusive = endDateInclusive.ToString("o"), endDateKind = endDate.Value.Kind.ToString() }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
                // #endregion
            }
            else
            {
                // #region agent log
                try { System.IO.File.AppendAllText(@"d:\Workspaces\SilkRoad\TunNetCom-SilkRoadErp\.cursor\debug.log", Newtonsoft.Json.JsonConvert.SerializeObject(new { sessionId = "debug-session", runId = "run1", hypothesisId = "H3", location = "InvoiceBaseInfosController.cs:88", message = "EndDate is NULL", data = new { }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
                // #endregion
            }

            if (customerId.HasValue)
            {
                _logger.LogInformation("Applying customerId filter: {CustomerId}", customerId.Value);
                baseQuery = baseQuery.Where(f => f.IdClient == customerId.Value);
            }

            // Apply tag filter if provided
            if (tagIds != null && tagIds.Any())
            {
                baseQuery = baseQuery.Where(f => _context.DocumentTag
                    .Any(dt => dt.DocumentType == DocumentTypes.Facture 
                        && dt.DocumentId == f.Num 
                        && tagIds.Contains(dt.TagId)));
            }

            // Build query with joins
            var queryWithJoins = from f in baseQuery
                                join c in _context.Client on f.IdClient equals c.Id
                                join bdl in _context.BonDeLivraison on f.Num equals bdl.NumFacture into deliveryNotes
                                from bdl in deliveryNotes.DefaultIfEmpty()
                                select new { f, c, bdl };

            // Log SQL query for debugging
            var sqlQuery = queryWithJoins.ToQueryString();
            _logger.LogInformation("SQL Query: {SqlQuery}", sqlQuery);
            
            // Load data to avoid SQL conversion issues with Statut (string -> enum -> int)
            var invoicesData = await queryWithJoins.ToListAsync(cancellationToken);
            _logger.LogInformation("Found {Count} invoices matching filters", invoicesData.Count);
            
            // #region agent log
            var invoiceDates = invoicesData.Select(x => new { x.f.Num, x.f.Date }).Distinct().Take(10).ToList();
            try { System.IO.File.AppendAllText(@"d:\Workspaces\SilkRoad\TunNetCom-SilkRoadErp\.cursor\debug.log", Newtonsoft.Json.JsonConvert.SerializeObject(new { sessionId = "debug-session", runId = "run2", hypothesisId = "H6", location = "InvoiceBaseInfosController.cs:100", message = "Actual invoice dates in result", data = new { invoiceDates = invoiceDates.Select(i => new { i.Num, Date = i.Date.ToString("o"), DateKind = i.Date.Kind.ToString() }) }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
            // #endregion

            // Group and calculate in memory, then return as IQueryable for OData filtering
            var invoicesQuery = invoicesData
                .GroupBy(x => new { x.f.Num, x.f.Date, x.f.IdClient, x.c.Nom, x.f.Statut })
                .Select(g => new InvoiceBaseInfo
                {
                    Number = g.Key.Num,
                    Date = new DateTimeOffset(g.Key.Date, TimeSpan.Zero),
                    CustomerId = g.Key.IdClient,
                    CustomerName = g.Key.Nom,
                    NetAmount = g.Where(x => x.bdl != null).Sum(x => x.bdl!.NetPayer) + timbre,
                    VatAmount = g.Where(x => x.bdl != null).Sum(x => x.bdl!.TotTva),
                    Statut = (int)g.Key.Statut,
                    StatutLibelle = g.Key.Statut.ToString()
                })
                .AsQueryable();

            _logger.LogInformation("Returning query for {Count} invoices", invoicesQuery.Count());
            return Ok(invoicesQuery);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in InvoiceBaseInfosController.Get");
            return StatusCode(500, "Internal server error");
        }
    }
}

