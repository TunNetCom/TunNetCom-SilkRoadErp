using TunNetCom.SilkRoadErp.Sales.Contracts.PrintHistory;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.PrintHistory.GetPrintHistory;

public class GetPrintHistoryQueryHandler(
    SalesContext _context,
    ILogger<GetPrintHistoryQueryHandler> _logger)
    : IRequestHandler<GetPrintHistoryQuery, PagedList<PrintHistoryResponse>>
{
    public async Task<PagedList<PrintHistoryResponse>> Handle(GetPrintHistoryQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Fetching print history with filters: DocumentType={DocumentType}, DocumentId={DocumentId}, DateFrom={DateFrom}, DateTo={DateTo}, UserId={UserId}, PrintMode={PrintMode}",
            request.DocumentType, request.DocumentId, request.DateFrom, request.DateTo, request.UserId, request.PrintMode);

        var query = _context.Set<Domain.Entites.PrintHistory>().AsNoTracking().AsQueryable();

        // Apply filters
        if (!string.IsNullOrEmpty(request.DocumentType))
        {
            query = query.Where(p => p.DocumentType == request.DocumentType);
        }

        if (request.DocumentId.HasValue)
        {
            query = query.Where(p => p.DocumentId == request.DocumentId.Value);
        }

        if (request.DateFrom.HasValue)
        {
            query = query.Where(p => p.PrintedAt >= request.DateFrom.Value);
        }

        if (request.DateTo.HasValue)
        {
            query = query.Where(p => p.PrintedAt <= request.DateTo.Value);
        }

        if (request.UserId.HasValue)
        {
            query = query.Where(p => p.UserId == request.UserId.Value);
        }

        if (!string.IsNullOrEmpty(request.PrintMode))
        {
            if (Enum.TryParse<PrintModeEnum>(request.PrintMode, true, out var printMode))
            {
                query = query.Where(p => p.PrintMode == printMode);
            }
        }

        // Order by printed at descending (most recent first)
        query = query.OrderByDescending(p => p.PrintedAt);

        // Project to response DTO
        var printHistoryQuery = query.Select(p => new PrintHistoryResponse
        {
            Id = p.Id,
            DocumentType = p.DocumentType,
            DocumentId = p.DocumentId,
            PrintMode = p.PrintMode.ToString(),
            UserId = p.UserId,
            Username = p.Username,
            PrintedAt = p.PrintedAt,
            PrinterName = p.PrinterName,
            Copies = p.Copies,
            FileName = p.FileName,
            IsDuplicata = p.IsDuplicata
        });

        var pagedPrintHistory = await PagedList<PrintHistoryResponse>.ToPagedListAsync(
            printHistoryQuery,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        _logger.LogInformation("Retrieved {Count} print history records", pagedPrintHistory.Items.Count);

        return pagedPrintHistory;
    }
}