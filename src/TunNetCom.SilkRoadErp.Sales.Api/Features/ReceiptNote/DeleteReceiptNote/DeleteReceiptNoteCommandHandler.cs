using Microsoft.EntityFrameworkCore;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ReceiptNote.DeleteReceiptNote;

public class DeleteReceiptNoteCommandHandler(SalesContext _context, ILogger<DeleteReceiptNoteCommandHandler> _logger) : IRequestHandler<DeleteReceiptNoteCommand, Result>
{
    public async Task<Result> Handle(DeleteReceiptNoteCommand deleteReceiptNoteCommand, CancellationToken cancellationToken)
    {
        _logger.LogEntityDeletionAttempt(nameof(BonDeReception), deleteReceiptNoteCommand.Num);

        var receiptnote = await _context.BonDeReception
            .FirstOrDefaultAsync(b => b.Num == deleteReceiptNoteCommand.Num, cancellationToken);

        if (receiptnote is null)
        {
            _logger.LogEntityNotFound(nameof(BonDeReception), deleteReceiptNoteCommand.Num);

            return Result.Fail(EntityNotFound.Error());
        }

        _ = _context.BonDeReception.Remove(receiptnote);
        _ = await _context.SaveChangesAsync(cancellationToken);

        _logger.LogEntityDeleted(nameof(BonDeReception),deleteReceiptNoteCommand.Num);

        return Result.Ok();
    }
}
