using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using TunNetCom.SilkRoadErp.Sales.Api.Features.AppParameters.GetAppParameters;

namespace TunNetCom.SilkRoadErp.Sales.Api.Controllers;

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
    public async Task<IActionResult> Get()
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
                               group new { f, c, bdl } by new { f.Num, f.Date, f.IdClient, c.Nom } into g
                               select new InvoiceBaseInfo
                               {
                                   Number = g.Key.Num,
                                   Date = new DateTimeOffset(g.Key.Date, TimeSpan.Zero),
                                   CustomerId = g.Key.IdClient,
                                   CustomerName = g.Key.Nom,
                                   NetAmount = g.Where(x => x.bdl != null).Sum(x => x.bdl!.NetPayer) + timbre,
                                   VatAmount = g.Where(x => x.bdl != null).Sum(x => x.bdl!.TotTva)
                               };

            return Ok(invoicesQuery.AsNoTracking());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in InvoiceBaseInfosController.Get");
            return StatusCode(500, "Internal server error");
        }
    }
}

