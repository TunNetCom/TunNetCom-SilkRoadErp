namespace TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.CreateDeliveryNote;

public class CreateDeliveryNoteCommandHandler(
    SalesContext _context,
    ILogger<CreateDeliveryNoteCommandHandler> _logger)
    : IRequestHandler<CreateDeliveryNoteCommand, Result<int>>
{
    public async Task<Result<int>> Handle(
        CreateDeliveryNoteCommand createDeliveryNoteCommand,
        CancellationToken cancellationToken) 
    {
        _logger.LogEntityCreated(nameof(BonDeLivraison), createDeliveryNoteCommand);

        //TODO add checks
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

        foreach(var deliveryNoteDetail in createDeliveryNoteCommand.DeliveryNoteDetails) 
        {
            var lignesBl = new LigneBl
            {
                RefProduit = deliveryNoteDetail.RefProduit,
                DesignationLi = deliveryNoteDetail.DesignationLi,
                QteLi = deliveryNoteDetail.QteLi,
                PrixHt = deliveryNoteDetail.PrixHt,
                Remise = deliveryNoteDetail.Remise,
                TotHt = deliveryNoteDetail.TotHt,
                Tva = deliveryNoteDetail.Tva,
                TotTtc = deliveryNoteDetail.TotTtc,
                NumBlNavigation = deliveryNote
            };

            // TODO make method to add lignesBl
            deliveryNote.LigneBl.Add( lignesBl );
        }

        _context.BonDeLivraison.Add(deliveryNote);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogEntityCreatedSuccessfully(nameof(BonDeLivraison), deliveryNote.Num);

        return deliveryNote.Num;
    }
}
