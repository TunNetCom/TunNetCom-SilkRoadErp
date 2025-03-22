namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ReceiptNote.GetReceiptNoteById;

public class GetReceiptNoteByIdQueryHandler(
    SalesContext _context,
    ILogger<GetReceiptNoteByIdQueryHandler> _logger)
    : IRequestHandler<GetReceiptNoteByIdQuery, Result<ReceiptNoteResponse>>
{
    public async Task<Result<ReceiptNoteResponse>> Handle(GetReceiptNoteByIdQuery getReceiptNoteByIdQuery, CancellationToken cancellationToken)
    {
        _logger.LogFetchingEntityById(nameof(BonDeReception), getReceiptNoteByIdQuery.Num);
        var receiptnote = await _context.BonDeReception.FindAsync(getReceiptNoteByIdQuery.Num, cancellationToken);
        if (receiptnote is null)
        {
            _logger.LogEntityNotFound(nameof(BonDeReception), getReceiptNoteByIdQuery.Num);

            return Result.Fail(EntityNotFound.Error());
        }
        _logger.LogEntityFetchedById(nameof(BonDeReception), getReceiptNoteByIdQuery.Num);
        return receiptnote.Adapt<ReceiptNoteResponse>();
    }
}
