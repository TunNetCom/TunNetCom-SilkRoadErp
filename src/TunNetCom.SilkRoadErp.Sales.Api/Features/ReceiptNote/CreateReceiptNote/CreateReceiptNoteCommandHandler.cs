namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ReceiptNote.CreateReceiptNote;

public class CreateReceiptNoteCommandHandler(
    SalesContext _context,
    ILogger<CreateReceiptNoteCommandHandler> _logger)
    : IRequestHandler<CreateReceiptNoteCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateReceiptNoteCommand createReceiptNoteCommand, CancellationToken cancellationToken)
    {
        _logger.LogEntityCreated(nameof(BonDeReception), createReceiptNoteCommand);

        var providerExists = await _context.Fournisseur.AnyAsync(p => p.Id == createReceiptNoteCommand.IdFournisseur, cancellationToken);
        if (!providerExists)
        {
            return Result.Fail("not_found");
        }


        var isReceiptNoteExist = await _context.BonDeReception.AnyAsync(
            Rec => Rec.Num == createReceiptNoteCommand.Num,
            cancellationToken);

        if (isReceiptNoteExist)
        {
            return Result.Fail("receiptnote_number_exists");
        }

        var ReceiptNote = BonDeReception.CreateReceiptNote(
            createReceiptNoteCommand.Num,
            createReceiptNoteCommand.NumBonFournisseur,
            createReceiptNoteCommand.DateLivraison,
            createReceiptNoteCommand.IdFournisseur,
            createReceiptNoteCommand.Date,
            createReceiptNoteCommand.NumFactureFournisseur
);


        _ = _context.BonDeReception.Add(ReceiptNote);
        _ = await _context.SaveChangesAsync(cancellationToken);

        _logger.LogEntityCreatedSuccessfully(nameof(BonDeReception), ReceiptNote.Num);

        return ReceiptNote.Num;
    }
}