using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using TunNetCom.SilkRoadErp.Sales.Contracts.RecieptNotes;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Controllers;

[Authorize]
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
    public async Task<IActionResult> Get(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] int? providerId = null,
        [FromQuery] List<int>? tagIds = null)
    {
        try
        {
            _logger.LogInformation("ReceiptNoteBaseInfosController.Get called with startDate: {StartDate}, endDate: {EndDate}, providerId: {ProviderId}, tagIds: {TagIds}", 
                startDate, endDate, providerId, tagIds != null ? string.Join(",", tagIds) : "null");

            // Build base query with filters before projection
            var baseQuery = _context.BonDeReception
                .AsNoTracking()
                .FilterByActiveAccountingYear();

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

            // Apply tag filter if provided (OR logic: document must have at least one of the selected tags)
            if (tagIds != null && tagIds.Any())
            {
                baseQuery = baseQuery.Where(br => _context.DocumentTag
                    .Any(dt => dt.DocumentType == "BonDeReception" 
                        && dt.DocumentId == br.Num 
                        && tagIds.Contains(dt.TagId)));
            }

            // Load data first to avoid SQL conversion issues with Statut (string -> enum -> int)
            var receiptNotes = await (from br in baseQuery
                                     join f in _context.Fournisseur on br.IdFournisseur equals f.Id into providerGroup
                                     from f in providerGroup.DefaultIfEmpty()
                                     select new { br, f })
                                     .ToListAsync();

            // Map to DTO in memory to avoid SQL conversion issues
            var receiptNoteQuery = receiptNotes
                .Select(x => new ReceiptNoteBaseInfo
                {
                    Number = x.br.Num,
                    Date = new DateTimeOffset(x.br.Date, TimeSpan.Zero),
                    ProviderId = x.br.IdFournisseur,
                    ProviderName = x.f != null ? x.f.Nom : null,
                    NetAmount = x.br.NetPayer,
                    GrossAmount = x.br.TotHTva,
                    VatAmount = x.br.TotTva,
                    Statut = (int)x.br.Statut,
                    StatutLibelle = x.br.Statut.ToString(),
                    SupplierReceiptNumber = x.br.NumBonFournisseur
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