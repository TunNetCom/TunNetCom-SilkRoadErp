namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ReceiptNote.UpdateReceiptNoteWithLines;

public record UpdateReceiptNoteWithLinesCommand(
    int Num,
    long NumBonFournisseur,
    DateTime DateLivraison,
    int IdFournisseur,
    DateTime Date,
    int? NumFactureFournisseur,
    List<ReceiptNoteLignes> ReceiptNoteLines
) : IRequest<Result>;

public record class ReceiptNoteLignes(
    string ProductRef,
    string ProductDescription,
    int Quantity,
    decimal UnitPrice,
    double Discount,
    double Tax);

