using FluentResults;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.PrintHistory.CreatePrintHistory;

public class CreatePrintHistoryCommandHandler(
    SalesContext _context,
    ILogger<CreatePrintHistoryCommandHandler> _logger)
    : IRequestHandler<CreatePrintHistoryCommand, Result<long>>
{
    public async Task<Result<long>> Handle(CreatePrintHistoryCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Creating print history record: DocumentType={DocumentType}, DocumentId={DocumentId}, PrintMode={PrintMode}, IsDuplicata={IsDuplicata}",
            request.DocumentType, request.DocumentId, request.PrintMode, request.IsDuplicata);

        var printHistory = Domain.Entites.PrintHistory.Create(
            documentType: request.DocumentType,
            documentId: request.DocumentId,
            printMode: request.PrintMode,
            userId: request.UserId,
            username: request.Username,
            printerName: request.PrinterName,
            copies: request.Copies,
            fileName: request.FileName,
            isDuplicata: request.IsDuplicata);

        await _context.PrintHistory.AddAsync(printHistory, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Print history record created with Id={Id}", printHistory.Id);

        return Result.Ok(printHistory.Id);
    }
}

