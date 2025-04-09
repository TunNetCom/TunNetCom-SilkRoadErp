using TunNetCom.SilkRoadErp.Sales.Contracts.DeliveryNote.Responses;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ReceiptNote.GetReceiptNoteWithDetails;

public class GetReceiptNoteWithDetailsQueryHandler(
    SalesContext _context,
    ILogger<GetReceiptNoteWithDetailsQueryHandler> _logger) :
    IRequestHandler<GetReceiptNoteWithDetailsQuery, ReceiptNotesWithSummary>
{
    private const string NumColumnName = "Num";
    private const string DateColumnName = "Date";
    private const string NetAmountColumnName = "TotTTC";

    public async Task<ReceiptNotesWithSummary> Handle(
        GetReceiptNoteWithDetailsQuery query,
        CancellationToken cancellationToken)
    {
        // Validate query parameters
        if (query.queryStringParameters.PageNumber < 1 || query.queryStringParameters.PageSize < 1)
        {
            throw new ArgumentException("PageNumber and PageSize must be greater than 0.");
        }

        // Build the base query
        var receiptNotesQuery = _context.ReceiptNoteView
            .Where(b => b.IdFournisseur == query.IdFournisseur)
            .Select(b => new ReceiptNoteDetailsResponse
            {
                Num = b.Num,
                NumBonFournisseur = b.NumBonFournisseur,
                DateLivraison = b.DateLivraison,
                IdFournisseur = b.IdFournisseur,
                Date = b.Date,
                NumFactureFournisseur = b.NumFactureFournisseur,
                TotTTC = b.TotalTTC,
                TotHTva = b.TotHt,
                TotTva = b.TotTva,
            })
            .AsQueryable();

        // Apply sorting if specified
        if (!string.IsNullOrEmpty(query.queryStringParameters.SortProprety) &&
            !string.IsNullOrEmpty(query.queryStringParameters.SortOrder))
        {
            _logger.LogInformation(
                "Sorting receipt notes by column: {Column}, order: {Order}",
                query.queryStringParameters.SortProprety,
                query.queryStringParameters.SortOrder);
            receiptNotesQuery = ApplySorting(
                receiptNotesQuery,
                query.queryStringParameters.SortProprety,
                query.queryStringParameters.SortOrder);
        }

        if(query.IsInvoiced && query.InvoiceId.HasValue)
        {
            receiptNotesQuery = receiptNotesQuery
                .Where(b => b.NumFactureFournisseur == query.InvoiceId);
        }

         if(query.IsInvoiced == false)
        {
            receiptNotesQuery = receiptNotesQuery
                .Where(b => b.NumFactureFournisseur == null);
        }


        // Get paged result
        var pagedResult = await PagedList<ReceiptNoteDetailsResponse>.ToPagedListAsync(
            receiptNotesQuery,
            query.queryStringParameters.PageNumber,
            query.queryStringParameters.PageSize,
            cancellationToken);

        // Materialize the full result set for totals (without pagination)
        var allReceiptNotes = await receiptNotesQuery.ToListAsync(cancellationToken);

        var totalGrossAmount = allReceiptNotes.Sum(b => b.TotHTva);
        var totalNetAmount = allReceiptNotes.Sum(b => b.TotTTC);
        var totalVATAmount = allReceiptNotes.Sum(b => b.TotTva);

        _logger.LogInformation(
            "Retrieved {Count} receipt notes for page {PageNumber} with page size {PageSize}",
            pagedResult.TotalCount,
            query.queryStringParameters.PageNumber,
            query.queryStringParameters.PageSize);

        return new ReceiptNotesWithSummary
        {
            ReceiptNotes = pagedResult,
            TotalGrossAmount = totalGrossAmount,
            TotalNetAmount = totalNetAmount,
            TotalVATAmount = totalVATAmount
        };
    }

    private IQueryable<ReceiptNoteDetailsResponse> ApplySorting(
IQueryable<ReceiptNoteDetailsResponse> invoiceQuery,
string sortProperty,
string sortOrder)
    {
        return SortQuery(invoiceQuery, sortProperty, sortOrder);
    }

    private IQueryable<ReceiptNoteDetailsResponse> SortQuery(
        IQueryable<ReceiptNoteDetailsResponse> query,
        string property,
        string order)
    {
        return (property, order) switch
        {
            (NumColumnName, SortConstants.Ascending) => query.OrderBy(d => d.Num),
            (NumColumnName, SortConstants.Descending) => query.OrderByDescending(d => d.Num),
            (NetAmountColumnName, SortConstants.Ascending) => query.OrderBy(d => d.TotTTC),
            (NetAmountColumnName, SortConstants.Descending) => query.OrderByDescending(d => d.TotTTC),
            (DateColumnName, SortConstants.Ascending) => query.OrderBy(d => d.Date),
            (DateColumnName, SortConstants.Descending) => query.OrderByDescending(d => d.Date),
            _ => query
        };
    }
}

//TODO data validation and checks 
