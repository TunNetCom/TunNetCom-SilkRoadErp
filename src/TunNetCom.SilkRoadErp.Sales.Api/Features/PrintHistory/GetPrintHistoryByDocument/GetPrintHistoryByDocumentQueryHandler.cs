using TunNetCom.SilkRoadErp.Sales.Contracts.PrintHistory;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.PrintHistory.GetPrintHistoryByDocument;

public class GetPrintHistoryByDocumentQueryHandler(
    SalesContext _context,
    ILogger<GetPrintHistoryByDocumentQueryHandler> _logger)
    : IRequestHandler<GetPrintHistoryByDocumentQuery, List<PrintHistoryResponse>>
{
    public async Task<List<PrintHistoryResponse>> Handle(GetPrintHistoryByDocumentQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching print history for document: {DocumentType} with ID: {DocumentId}",
            request.DocumentType, request.DocumentId);

        var printHistory = await _context.Set<Domain.Entites.PrintHistory>()
            .AsNoTracking()
            .Where(p => p.DocumentType == request.DocumentType && p.DocumentId == request.DocumentId)
            .OrderByDescending(p => p.PrintedAt)
            .ToListAsync(cancellationToken);

        var response = printHistory.Select(p => new PrintHistoryResponse
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
        }).ToList();

        _logger.LogInformation("Retrieved {Count} print history records for document {DocumentType}:{DocumentId}",
            response.Count, request.DocumentType, request.DocumentId);

        return response;
    }
}


