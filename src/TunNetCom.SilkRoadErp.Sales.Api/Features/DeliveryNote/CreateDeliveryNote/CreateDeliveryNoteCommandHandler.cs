using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.CreateDeliveryNote;

public class CreateDeliveryNoteCommandHandler(
    SalesContext _context,
    ILogger<CreateDeliveryNoteCommandHandler> _logger,
    INumberGeneratorService _numberGeneratorService)
    : IRequestHandler<CreateDeliveryNoteCommand, Result<int>>
{
    public async Task<Result<int>> Handle(
        CreateDeliveryNoteCommand createDeliveryNoteCommand,
        CancellationToken cancellationToken) 
    {
        _logger.LogEntityCreated(nameof(BonDeLivraison), createDeliveryNoteCommand);

        //TODO add checks
        // Get the active accounting year
        var activeAccountingYear = await _context.AccountingYear
            .FirstOrDefaultAsync(ay => ay.IsActive, cancellationToken);

        if (activeAccountingYear == null)
        {
            _logger.LogError("No active accounting year found");
            return Result.Fail("no_active_accounting_year");
        }

        var num = await _numberGeneratorService.GenerateBonDeLivraisonNumberAsync(activeAccountingYear.Id, cancellationToken);

        var deliveryNote = BonDeLivraison.CreateBonDeLivraison
            (
                createDeliveryNoteCommand.Date,
                createDeliveryNoteCommand.TotHTva,
                createDeliveryNoteCommand.TotTva,
                createDeliveryNoteCommand.NetPayer,
                createDeliveryNoteCommand.TempBl,
                createDeliveryNoteCommand.NumFacture,
                createDeliveryNoteCommand.ClientId,
                activeAccountingYear.Id
            );
        deliveryNote.Num = num;

        var deliveryNoteDetailsList = createDeliveryNoteCommand.DeliveryNoteDetails?.ToList() ?? new List<LigneBlSubCommand>();
        _logger.LogInformation($"Creating delivery note with {deliveryNoteDetailsList.Count} items");

        foreach(var deliveryNoteDetail in deliveryNoteDetailsList) 
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

        _logger.LogInformation($"Added {deliveryNote.LigneBl.Count} lines to delivery note before saving");

        _ = _context.BonDeLivraison.Add(deliveryNote);
        _ = await _context.SaveChangesAsync(cancellationToken);

        _logger.LogEntityCreatedSuccessfully(nameof(BonDeLivraison), deliveryNote.Num);

        return deliveryNote.Num;
    }
}
