using TunNetCom.SilkRoadErp.Sales.Contracts.DeliveryNote.Responses;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.GetDeliveryNotesBaseInfosWithSummaries;

public class GetDeliveryNotesBaseInfosWithSummariesQueryHandler(
    SalesContext _context,
    ILogger<GetDeliveryNotesBaseInfosWithSummariesQueryHandler> _logger)
    : IRequestHandler<GetDeliveryNotesBaseInfosWithSummariesQuery, GetDeliveryNotesWithSummariesResponse>
{
        private const string _numColumName = nameof(GetDeliveryNoteBaseInfos.Number);
        private const string _netAmountColumnName = nameof(GetDeliveryNoteBaseInfos.NetAmount);
        private const string _grossAmountColumnName = nameof(GetDeliveryNoteBaseInfos.GrossAmount);
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
                Number = t.Num,
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
            _logger.LogInformation(
                "sorting delivery notes column : {column} order : {order}",
                request.SortProperty,
                request.SortOrder);
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

        _logger.LogEntitiesFetched(nameof(BonDeLivraison), pagedDeliveryNote.Items.Count);

        return getDeliveryNotesWithSummariesResponse;
    }

    private IQueryable<GetDeliveryNoteBaseInfos> ApplySorting(
        IQueryable<GetDeliveryNoteBaseInfos> deliveryNoteQuery,
        string sortProperty,
        string sortOrder)
    {
        return SortQuery(deliveryNoteQuery, sortProperty, sortOrder);
    }

    private IQueryable<GetDeliveryNoteBaseInfos> SortQuery(
        IQueryable<GetDeliveryNoteBaseInfos> query,
        string property,
        string order)
    {
        return (property, order) switch
        {
            (_numColumName, SortConstants.Ascending) => query.OrderBy(d => d.Number),
            (_numColumName, SortConstants.Descending) => query.OrderByDescending(d => d.Number),
            (_netAmountColumnName, SortConstants.Ascending) => query.OrderBy(d => d.NetAmount),
            (_netAmountColumnName, SortConstants.Descending) => query.OrderByDescending(d => d.NetAmount),
            (_grossAmountColumnName, SortConstants.Ascending) => query.OrderBy(d => d.GrossAmount),
            (_grossAmountColumnName, SortConstants.Descending) => query.OrderByDescending(d => d.GrossAmount),
            _ => query
        };
    }
}
