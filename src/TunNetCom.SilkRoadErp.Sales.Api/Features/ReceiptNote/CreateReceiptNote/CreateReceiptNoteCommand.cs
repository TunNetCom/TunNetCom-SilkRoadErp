namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ReceiptNote.CreateReceiptNote;

public record CreateReceiptNoteCommand(
    int Num,
    long NumBonFournisseur,
    DateTime DateLivraison,
    int IdFournisseur,
    DateTime Date,
    int? NumFactureFournisseur
    ):IRequest<Result<int>>;