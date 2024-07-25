namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Invoices.GetInvoicesByClient;

public class GetInvoicesByClientQueryHandler(
    SalesContext _context,
    AutoMapperIMapper _mapper)
    : IRequestHandler<GetInvoicesByClientQuery, Result<PagedList<InvoiceResponse>>>
{
    public async Task<Result<PagedList<InvoiceResponse>>> Handle(GetInvoicesByClientQuery query, CancellationToken cancellationToken)
    {
        var invoicesQuery = _context.Facture
            .Where(f => f.IdClient == query.ClientId)
            .ProjectTo<InvoiceResponse>(_mapper.ConfigurationProvider);

        var pagedInvoices = await PagedList<InvoiceResponse>.ToPagedListAsync(invoicesQuery, query.PageNumber, query.PageSize, cancellationToken);

        return Result.Ok(pagedInvoices);
    }
}
