namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ReceiptNote.DeleteReceiptNote;

public class DeleteReceiptNoteCommandHandler(SalesContext _context, ILogger<DeleteReceiptNoteCommandHandler> _logger) : IRequestHandler<DeleteReceiptNoteCommand, Result>
{
    public async Task<Result> Handle(DeleteReceiptNoteCommand deleteReceiptNoteCommand, CancellationToken cancellationToken)
    {
        _logger.LogEntityDeletionAttempt(nameof(BonDeReception), deleteReceiptNoteCommand.Num);

        var receiptnote = await _context.BonDeReception.FindAsync(deleteReceiptNoteCommand.Num);

        if (receiptnote is null)
        {
            _logger.LogEntityNotFound(nameof(BonDeReception), deleteReceiptNoteCommand.Num);

            return Result.Fail("receiptnote_not_found");
        }

        _context.BonDeReception.Remove(receiptnote);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogEntityDeleted(nameof(BonDeReception),deleteReceiptNoteCommand.Num);

        return Result.Ok();
    }
}
