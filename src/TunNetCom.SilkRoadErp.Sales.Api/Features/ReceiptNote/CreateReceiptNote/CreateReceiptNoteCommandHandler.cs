using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ReceiptNote.CreateReceiptNote;

public class CreateReceiptNoteCommandHandler(
    SalesContext _context,
    ILogger<CreateReceiptNoteCommandHandler> _logger,
    INumberGeneratorService _numberGeneratorService)
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

        // Get the active accounting year
        var activeAccountingYear = await _context.AccountingYear
            .FirstOrDefaultAsync(ay => ay.IsActive, cancellationToken);

        if (activeAccountingYear == null)
        {
            _logger.LogError("No active accounting year found");
            return Result.Fail("no_active_accounting_year");
        }

        var num = await _numberGeneratorService.GenerateBonDeReceptionNumberAsync(activeAccountingYear.Id, cancellationToken);

        var ReceiptNote = BonDeReception.CreateReceiptNote(
            num,
            createReceiptNoteCommand.NumBonFournisseur,
            createReceiptNoteCommand.DateLivraison,
            createReceiptNoteCommand.IdFournisseur,
            createReceiptNoteCommand.Date,
            createReceiptNoteCommand.NumFactureFournisseur,
            activeAccountingYear.Id
);

        _ = _context.BonDeReception.Add(ReceiptNote);
        _ = await _context.SaveChangesAsync(cancellationToken);

        _logger.LogEntityCreatedSuccessfully(nameof(BonDeReception), ReceiptNote.Id);

        return ReceiptNote.Id;
    }
}
