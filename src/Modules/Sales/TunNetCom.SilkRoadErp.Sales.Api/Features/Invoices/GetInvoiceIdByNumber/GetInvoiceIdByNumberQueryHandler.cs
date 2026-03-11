using Microsoft.EntityFrameworkCore;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Invoices.GetInvoiceIdByNumber;

public class GetInvoiceIdByNumberQueryHandler(
    SalesContext _context,
    ILogger<GetInvoiceIdByNumberQueryHandler> _logger)
    : IRequestHandler<GetInvoiceIdByNumberQuery, Result<int>>
{
    public async Task<Result<int>> Handle(
        GetInvoiceIdByNumberQuery query,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching invoice ID by number {Number}", query.Number);

        var invoiceId = await _context.Facture
            .AsNoTracking()
            .Where(f => f.Num == query.Number)
            .Select(f => f.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (invoiceId == 0)
        {
            _logger.LogWarning("Invoice with number {Number} not found", query.Number);
            return Result.Fail("invoice_not_found");
        }

        _logger.LogInformation("Invoice ID {Id} found for number {Number}", invoiceId, query.Number);
        return Result.Ok(invoiceId);
    }
}
