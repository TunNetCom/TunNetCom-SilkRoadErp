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

    public InvoiceBaseInfosController(
        SalesContext context,
        ILogger<InvoiceBaseInfosController> logger,
        IMediator mediator)
    {
        _context = context;
        _logger = logger;
        _mediator = mediator;
    }

    [EnableQuery(MaxExpansionDepth = 3, MaxAnyAllExpressionDepth = 3)]
    public async Task<IActionResult> Get([FromQuery] List<int>? tagIds = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var appParams = await _mediator.Send(new GetAppParametersQuery());
            var timbre = appParams.Value.Timbre;

            // Build base query
            var baseQuery = from f in _context.Facture
                           join c in _context.Client on f.IdClient equals c.Id
                           join bdl in _context.BonDeLivraison on f.Num equals bdl.NumFacture into deliveryNotes
                           from bdl in deliveryNotes.DefaultIfEmpty()
                           select new { f, c, bdl };

            // Apply tag filter if provided (before loading to memory)
            if (tagIds != null && tagIds.Any())
            {
                var factureNumsWithTags = await _context.DocumentTag
                    .Where(dt => dt.DocumentType == DocumentTypes.Facture && tagIds.Contains(dt.TagId))
                    .Select(dt => dt.DocumentId)
                    .Distinct()
                    .ToListAsync(cancellationToken);
                
                baseQuery = baseQuery.Where(x => factureNumsWithTags.Contains(x.f.Num));
            }

            // Load data to avoid SQL conversion issues with Statut (string -> enum -> int)
            var invoicesData = await baseQuery.ToListAsync(cancellationToken);

            // Group and calculate in memory to avoid SQL conversion issues
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

            return Ok(invoicesQuery);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in InvoiceBaseInfosController.Get");
            return StatusCode(500, "Internal server error");
        }
    }
}

