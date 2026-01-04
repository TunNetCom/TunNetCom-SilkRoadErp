using TunNetCom.SilkRoadErp.Sales.Contracts;
using TunNetCom.SilkRoadErp.Sales.Contracts.DeliveryNote.Responses;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;
using TunNetCom.SilkRoadErp.Sales.Api.Exceptions;

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
        
        // Build base query with filters before projection (on entity properties)
        var baseQuery = _context.BonDeLivraison
            .AsNoTracking()
            .FilterByActiveAccountingYear()
            .AsQueryable();

        // Apply filters on entity before projection
        if (request.CustomerId.HasValue)
        {
            baseQuery = baseQuery.Where(bdl => bdl.ClientId == request.CustomerId.Value);
        }

        if (request.TechnicianId.HasValue)
        {
            _logger.LogInformation("Applying technician filter: {technicianId}", request.TechnicianId);
            baseQuery = baseQuery.Where(bdl => bdl.InstallationTechnicianId == request.TechnicianId.Value);
        }

        // Note: Status filter is applied after loading data to avoid SQL conversion issues
        // (Statut is stored as string 'Draft'/'Valid' in DB, not as int)

        // Apply tag filter if provided (OR logic: document must have at least one of the selected tags)
        if (request.TagIds != null && request.TagIds.Any())
        {
            _logger.LogInformation("Applying tag filter: {tagIds}", string.Join(",", request.TagIds));
            baseQuery = baseQuery.Where(bdl => _context.DocumentTag
                .Any(dt => dt.DocumentType == "BonDeLivraison" 
                    && dt.DocumentId == bdl.Num 
                    && request.TagIds.Contains(dt.TagId)));
        }

        // Apply Date Range filters
        if (request.StartDate.HasValue)
        {
            _logger.LogInformation("Applying start date filter: {startDate}", request.StartDate);
            baseQuery = baseQuery.Where(bdl => bdl.Date >= request.StartDate.Value);
        }
        if (request.EndDate.HasValue)
        {
            _logger.LogInformation("Applying end date filter: {endDate}", request.EndDate);
            var endDateInclusive = request.EndDate.Value.Date.AddDays(1).AddTicks(-1);
            baseQuery = baseQuery.Where(bdl => bdl.Date <= endDateInclusive);
        }

        // Load data first to avoid SQL conversion issues with Statut (string -> enum -> int)
        var deliveryNotesData = await (from bdl in baseQuery
                                      join c in _context.Client on bdl.ClientId equals c.Id
                                      select new { bdl, c })
                                      .ToListAsync(cancellationToken);

        // Apply Status filter in memory if needed
        if (request.Status.HasValue)
        {
            _logger.LogInformation("Applying status filter in memory: {status}", request.Status);
            var statusEnum = (DocumentStatus)request.Status.Value;
            deliveryNotesData = deliveryNotesData
                .Where(x => x.bdl.Statut == statusEnum)
                .ToList();
        }

        // Now project to DTO in memory
        var deliveryNoteQuery = deliveryNotesData
            .Select(x => new GetDeliveryNoteBaseInfos
            {
                Id = x.bdl.Id,
                Number = x.bdl.Num,
                Date = x.bdl.Date,
                NetAmount = x.bdl.NetPayer,
                CustomerName = x.c != null ? x.c.Nom : null,
                GrossAmount = x.bdl.TotHTva,
                VatAmount = x.bdl.TotTva,
                NumFacture = x.bdl.NumFacture,
                CustomerId = x.bdl.ClientId,
                Statut = (int)x.bdl.Statut,
                StatutLibelle = x.bdl.Statut.ToString()
            })
            .AsQueryable();

        // Apply invoice filters
        if (request.IsInvoiced.Equals(true) && !request.InvoiceId.HasValue)
        {
            deliveryNoteQuery = deliveryNoteQuery.Where(d => d.NumFacture.HasValue);
        }
        if (request.InvoiceId.HasValue && request.IsInvoiced.Equals(true))
        {
            deliveryNoteQuery = deliveryNoteQuery.Where(d => d.NumFacture == request.InvoiceId);
        }
        if (request.IsInvoiced.Equals(false))
        {
            deliveryNoteQuery = deliveryNoteQuery.Where(d => d.NumFacture == null);
        }

        // Apply SearchKeyword filter
        if (!string.IsNullOrEmpty(request.SearchKeyword))
        {
            _logger.LogInformation("Applying search keyword filter: {searchKeyword}", request.SearchKeyword);
            deliveryNoteQuery = deliveryNoteQuery.Where(
                d => d.Number.ToString().Contains(request.SearchKeyword)
                  || d.Date.ToString().Contains(request.SearchKeyword)
                  || (d.CustomerName != null && d.CustomerName.Contains(request.SearchKeyword))
                  || d.CustomerId.ToString().Contains(request.SearchKeyword)
                  || (d.NumFacture != null && d.NumFacture.ToString().Contains(request.SearchKeyword)));
        }

        // Convert to list for in-memory operations (sorting, totals, pagination)
        _logger.LogInformation("Converting to list for in-memory operations");
        var allFilteredData = deliveryNoteQuery.ToList();

        // Calculate totals from all filtered data (before pagination)
        _logger.LogInformation("Getting Gross, Vat, and Net amounts from filtered data");
        var totalGrossAmount = allFilteredData.Sum(d => d.GrossAmount);
        var totalVATAmount = allFilteredData.Sum(d => d.VatAmount);
        var totalNetAmount = allFilteredData.Sum(d => d.NetAmount);

        // Apply Sorting on in-memory list
        if (request.SortOrder != null && request.SortProperty != null)
        {
            _logger.LogInformation(
                "Sorting delivery notes column: {column} order: {order}",
                request.SortProperty,
                request.SortOrder);
            allFilteredData = ApplySortingToEnumerable(allFilteredData, request.SortProperty, request.SortOrder).ToList();
        }

        // Apply pagination manually (since we're working with in-memory data)
        var totalCount = allFilteredData.Count;
        var pageNumber = request.PageNumber;
        var pageSize = request.PageSize;
        
        if (pageNumber < 1 || pageSize < 1)
        {
            throw new InvalidPaginationParamsException();
        }

        var pagedItems = allFilteredData
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var pagedDeliveryNote = new PagedList<GetDeliveryNoteBaseInfos>(
            pagedItems,
            totalCount,
            pageNumber,
            pageSize);
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

    private IEnumerable<GetDeliveryNoteBaseInfos> ApplySortingToEnumerable(
        IEnumerable<GetDeliveryNoteBaseInfos> data,
        string sortProperty,
        string sortOrder)
    {
        return (sortProperty, sortOrder) switch
        {
            (_numColumName, SortConstants.Ascending) => data.OrderBy(d => d.Number),
            (_numColumName, SortConstants.Descending) => data.OrderByDescending(d => d.Number),
            (_netAmountColumnName, SortConstants.Ascending) => data.OrderBy(d => d.NetAmount),
            (_netAmountColumnName, SortConstants.Descending) => data.OrderByDescending(d => d.NetAmount),
            (_grossAmountColumnName, SortConstants.Ascending) => data.OrderBy(d => d.GrossAmount),
            (_grossAmountColumnName, SortConstants.Descending) => data.OrderByDescending(d => d.GrossAmount),
            _ => data
        };
    }
}