using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
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
    public async Task<IActionResult> Get([FromQuery] List<int>? tagIds = null)
    {
        try
        {
            var appParams = await _mediator.Send(new GetAppParametersQuery());
            var timbre = appParams.Value.Timbre;

            // Use grouping to calculate sums - EF Core can translate this to SQL
            var invoicesQuery = from f in _context.Facture
                               join c in _context.Client on f.IdClient equals c.Id
                               join bdl in _context.BonDeLivraison on f.Num equals bdl.NumFacture into deliveryNotes
                               from bdl in deliveryNotes.DefaultIfEmpty()
                               group new { f, c, bdl } by new { f.Num, f.Date, f.IdClient, c.Nom, f.Statut } into g
                               select new InvoiceBaseInfo
                               {
                                   Number = g.Key.Num,
                                   Date = new DateTimeOffset(g.Key.Date, TimeSpan.Zero),
                                   CustomerId = g.Key.IdClient,
                                   CustomerName = g.Key.Nom,
                                   NetAmount = g.Where(x => x.bdl != null).Sum(x => x.bdl!.NetPayer) + timbre,
                                   VatAmount = g.Where(x => x.bdl != null).Sum(x => x.bdl!.TotTva),
                                   Statut = (int)g.Key.Statut,
                                   StatutLibelle = g.Key.Statut == DocumentStatus.Brouillon ? "Brouillon" : "Validé"
                               };

            // Apply tag filter if provided (OR logic: document must have at least one of the selected tags)
            if (tagIds != null && tagIds.Any())
            {
                invoicesQuery = invoicesQuery.Where(inv => _context.DocumentTag
                    .Any(dt => dt.DocumentType == "Facture" 
                        && dt.DocumentId == inv.Number 
                        && tagIds.Contains(dt.TagId)));
            }

            return Ok(invoicesQuery.AsNoTracking());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in InvoiceBaseInfosController.Get");
            return StatusCode(500, "Internal server error");
        }
    }
}

