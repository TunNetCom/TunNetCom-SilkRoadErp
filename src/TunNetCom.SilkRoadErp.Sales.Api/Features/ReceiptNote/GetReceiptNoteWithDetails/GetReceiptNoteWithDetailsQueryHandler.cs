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
    private const string GrossAmountColumnName = "TotHTva";

    public async Task<ReceiptNotesWithSummary> Handle(
        GetReceiptNoteWithDetailsQuery query,
        CancellationToken cancellationToken)
    {
        // Validate query parameters
        if (query.QueryStringParameters.PageNumber < 1 || query.QueryStringParameters.PageSize < 1)
        {
            throw new ArgumentException("PageNumber and PageSize must be greater than 0.");
        }

        // Build the base query
        var receiptNotesQuery = _context.ReceiptNoteView
            .Where(b => b.IdFournisseur == query.IdFournisseur)
            .Where(b => b.NumFactureFournisseur == null) // Filter where NumFactureFournisseur is null
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
        if (!string.IsNullOrEmpty(query.QueryStringParameters.SortProprety) &&
            !string.IsNullOrEmpty(query.QueryStringParameters.SortOrder))
        {
            _logger.LogInformation(
                "Sorting receipt notes by column: {Column}, order: {Order}",
                query.QueryStringParameters.SortProprety,
                query.QueryStringParameters.SortOrder);
            receiptNotesQuery = ApplySorting(
                receiptNotesQuery,
                query.QueryStringParameters.SortProprety,
                query.QueryStringParameters.SortOrder);
        }

        // Get paged result
        var pagedResult = await PagedList<ReceiptNoteDetailsResponse>.ToPagedListAsync(
            receiptNotesQuery,
            query.QueryStringParameters.PageNumber,
            query.QueryStringParameters.PageSize,
            cancellationToken);

        // Materialize the full result set for totals (without pagination)
        var allReceiptNotes = await receiptNotesQuery.ToListAsync(cancellationToken);

        // Calculate totals
        var totalGrossAmount = allReceiptNotes.Sum(b => b.TotHTva);
        var totalNetAmount = allReceiptNotes.Sum(b => b.TotTTC);
        var totalVATAmount = allReceiptNotes.Sum(b => b.TotTva);

        _logger.LogInformation(
            "Retrieved {Count} receipt notes for page {PageNumber} with page size {PageSize}",
            pagedResult.TotalCount,
            query.QueryStringParameters.PageNumber,
            query.QueryStringParameters.PageSize);

        // Construct the response
        return new ReceiptNotesWithSummary
        {
            ReceiptNotes = pagedResult,
            TotalGrossAmount = totalGrossAmount,
            TotalNetAmount = totalNetAmount,
            TotalVATAmount = totalVATAmount
        };
    }

    private IQueryable<ReceiptNoteDetailsResponse> ApplySorting(
        IQueryable<ReceiptNoteDetailsResponse> query,
        string sortProperty,
        string sortOrder)
    {
        return (sortProperty.ToLower(), sortOrder.ToLower()) switch
        {
            (NumColumnName, "asc") => query.OrderBy(d => d.Num),
            (NumColumnName, "desc") => query.OrderByDescending(d => d.Num),
            (DateColumnName, "asc") => query.OrderBy(d => d.Date),
            (DateColumnName, "desc") => query.OrderByDescending(d => d.Date),
            (NetAmountColumnName, "asc") => query.OrderBy(d => d.TotTTC),
            (NetAmountColumnName, "desc") => query.OrderByDescending(d => d.TotTTC),
            (GrossAmountColumnName, "asc") => query.OrderBy(d => d.TotHTva),
            (GrossAmountColumnName, "desc") => query.OrderByDescending(d => d.TotHTva),
            _ => query // Default: no sorting if invalid property/order
        };
    }
}

//TODO sorting and total count and data validation and checks 
