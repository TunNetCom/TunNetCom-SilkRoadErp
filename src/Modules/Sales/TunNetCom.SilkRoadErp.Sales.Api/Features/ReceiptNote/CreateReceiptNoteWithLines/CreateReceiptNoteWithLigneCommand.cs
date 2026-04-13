namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ProviderReceiptNoteLine.CreateReceiptNoteWithLines;

internal record class CreateReceiptNoteWithLigneCommand(
        long NumBonFournisseur ,
        DateTime DateLivraison,
        int IdFournisseur ,
        DateTime Date ,
        int? NumFactureFournisseur ,
        List<ReceiptNoteLignes> ReceiptNoteLines
    ) : IRequest<Result<int>>;

internal record class ReceiptNoteLignes(
    string ProductRef,
    string ProductDescription,
    int Quantity,
    decimal UnitPrice,
    double Discount,
    double Tax);
