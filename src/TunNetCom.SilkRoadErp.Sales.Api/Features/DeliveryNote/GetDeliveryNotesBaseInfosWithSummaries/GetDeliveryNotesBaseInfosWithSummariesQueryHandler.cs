using Microsoft.Data.SqlClient;
using TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.GetDeliveryNote;
using TunNetCom.SilkRoadErp.Sales.Contracts.DeliveryNote.Responses;
namespace TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.GetDeliveryNotesBaseInfosWithSummaries;

public class GetDeliveryNotesBaseInfosWithSummariesQueryHandler(
    SalesContext _context,
    ILogger<GetDeliveryNoteQueryHandler> _logger)
    : IRequestHandler<GetDeliveryNotesBaseInfosWithSummariesQuery, GetDeliveryNotesWithSummariesResponse>
{
    public async Task<GetDeliveryNotesWithSummariesResponse> Handle(
        GetDeliveryNotesBaseInfosWithSummariesQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogPaginationRequest(nameof(BonDeLivraison), request.PageNumber, request.PageSize);

        var deliveryNoteQuery = _context.BonDeLivraison
            .Where(d => d.ClientId == request.CustomerId)
            .Select(t =>
            new GetDeliveryNoteBaseInfos
            {
                Num = t.Num,
                Date = t.Date,
                NetAmount = t.NetPayer,
                GrossAmount = t.TotHTva,
                VatAmount = t.TotTva,
                NumFacture = t.NumFacture
            })
            .AsQueryable();

        if (request.InvoiceId.HasValue && request.IsInvoiced)
        {
            deliveryNoteQuery = deliveryNoteQuery.Where(d => d.NumFacture == request.InvoiceId);
        }

        if(!request.IsInvoiced)
        {
            deliveryNoteQuery = deliveryNoteQuery.Where(d => d.NumFacture == null);
        }
        if(request.SortOrder != null && request.SortProperty != null)
        {
            _logger.LogInformation("sorting delivery notes column : {column} order : {order}", request.SortProperty, request.SortOrder);
            deliveryNoteQuery = ApplySorting(deliveryNoteQuery, request.SortProperty, request.SortOrder);
        }
        _logger.LogInformation("Getting Gross, Vat and Net amounts");
        var totalGrossAmount = await deliveryNoteQuery.SumAsync(d => d.GrossAmount, cancellationToken);
        var totalVATAmount = await deliveryNoteQuery.SumAsync(d => d.VatAmount, cancellationToken);
        var totalNetAmount = await deliveryNoteQuery.SumAsync(d => d.NetAmount, cancellationToken);

        var pagedDeliveryNote = await PagedList<GetDeliveryNoteBaseInfos>.ToPagedListAsync(
            deliveryNoteQuery,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        var getDeliveryNotesWithSummariesResponse = new GetDeliveryNotesWithSummariesResponse
        {
            GetDeliveryNoteBaseInfos = pagedDeliveryNote,
            TotalGrossAmount = totalGrossAmount,
            TotalNetAmount = totalNetAmount,
            TotalVatAmount = totalVATAmount
        };


        _logger.LogEntitiesFetched(nameof(BonDeLivraison), pagedDeliveryNote.Count);

        return getDeliveryNotesWithSummariesResponse;
    }

    private IQueryable<GetDeliveryNoteBaseInfos> ApplySorting(IQueryable<GetDeliveryNoteBaseInfos> deliveryNoteQuery, string sortProperty, string sortOrder)
    {
        return SortQuery(deliveryNoteQuery, sortProperty, sortOrder.ToLower());
    }

    private IQueryable<GetDeliveryNoteBaseInfos> SortQuery(IQueryable<GetDeliveryNoteBaseInfos> query, string property, string order)
    {
        return (property, order) switch
        {
            ("Num", "ascending") => query.OrderBy(d => d.Num),
            ("Num", "descending") => query.OrderByDescending(d => d.Num),
            ("NetAmount", "ascending") => query.OrderBy(d => d.NetAmount),
            ("NetAmount", "descending") => query.OrderByDescending(d => d.NetAmount),
            ("GrossAmount", "ascending") => query.OrderBy(d => d.GrossAmount),
            ("GrossAmount", "descending") => query.OrderByDescending(d => d.GrossAmount),
            _ => query
        };
    }
}
