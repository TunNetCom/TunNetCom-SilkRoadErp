namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ReceiptNote.UpdateReceiptNote;

public record UpdateReceiptNoteCommand(
    int Num,
    long NumBonFournisseur,
    DateTime DateLivraison,
    int IdFournisseur,
    DateTime Date,
    int? NumFactureFournisseur
    ) : IRequest<Result>;
