using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using TunNetCom.SilkRoadErp.Sales.Contracts.RetourMarchandiseFournisseur;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Controllers;

[Authorize]
public class RetourMarchandiseFournisseurBaseInfosController : ODataController
{
    private readonly SalesContext _context;
    private readonly ILogger<RetourMarchandiseFournisseurBaseInfosController> _logger;

    public RetourMarchandiseFournisseurBaseInfosController(
        SalesContext context,
        ILogger<RetourMarchandiseFournisseurBaseInfosController> logger)
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
            _logger.LogInformation("RetourMarchandiseFournisseurBaseInfosController.Get called with startDate: {StartDate}, endDate: {EndDate}, providerId: {ProviderId}, tagIds: {TagIds}", 
                startDate, endDate, providerId, tagIds);

            // Build base query with filters before projection
            var query = _context.RetourMarchandiseFournisseur.AsNoTracking();

            // Apply date filter if provided
            if (startDate.HasValue)
            {
                query = query.Where(r => r.Date >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(r => r.Date <= endDate.Value);
            }

            // Apply provider filter if provided
            if (providerId.HasValue)
            {
                query = query.Where(r => r.IdFournisseur == providerId.Value);
            }

            // Apply tag filter if provided (OR logic: document must have at least one of the selected tags)
            if (tagIds != null && tagIds.Any())
            {
                query = query.Where(r => _context.DocumentTag
                    .Any(dt => dt.DocumentType == "RetourMarchandiseFournisseur" 
                        && dt.DocumentId == r.Num 
                        && tagIds.Contains(dt.TagId)));
            }

            // Load data first to avoid SQL conversion issues with Statut (string -> enum -> int)
            var retours = await (from r in query
                                join f in _context.Fournisseur on r.IdFournisseur equals f.Id into providerGroup
                                from f in providerGroup.DefaultIfEmpty()
                                select new { r, f })
                                .ToListAsync();

            // Map to DTO in memory to avoid SQL conversion issues
            var retourQuery = retours
                .Select(x => new RetourMarchandiseFournisseurBaseInfo
                {
                    Number = x.r.Num,
                    Date = new DateTimeOffset(x.r.Date, TimeSpan.Zero),
                    ProviderId = x.r.IdFournisseur,
                    ProviderName = x.f != null ? x.f.Nom : null,
                    NetAmount = x.r.NetPayer,
                    GrossAmount = x.r.TotHTva,
                    VatAmount = x.r.TotTva,
                    Statut = (int)x.r.Statut,
                    StatutLibelle = x.r.Statut.ToString()
                })
                .AsQueryable();

            _logger.LogInformation("Returning query for retour marchandise fournisseur");
            return Ok(retourQuery);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in RetourMarchandiseFournisseurBaseInfosController.Get");
            return BadRequest(new { Error = ex.Message });
        }
    }
}

