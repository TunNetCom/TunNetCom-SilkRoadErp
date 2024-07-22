namespace TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.CreateDeliveryNote;

public class CreateDeliveryNoteCommandHandler(
    SalesContext _context,
    ILogger<CreateDeliveryNoteCommandHandler> _logger)
    : IRequestHandler<CreateDeliveryNoteCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateDeliveryNoteCommand createDeliveryNoteCommand, CancellationToken cancellationToken) 
    {
        _logger.LogEntityCreated("DeliveryNote", createDeliveryNoteCommand);

        var deliveryNote = BonDeLivraison.CreateBonDeLivraison
            (
                createDeliveryNoteCommand.Date,
                createDeliveryNoteCommand.TotHTva,
                createDeliveryNoteCommand.TotTva,
                createDeliveryNoteCommand.NetPayer,
                createDeliveryNoteCommand.TempBl,
                createDeliveryNoteCommand.NumFacture,
                createDeliveryNoteCommand.ClientId
            );
        _context.BonDeLivraison.Add(deliveryNote);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogEntityCreatedSuccessfully("DeliveryNote", deliveryNote.Num);

        return deliveryNote.Num;
    }
}
