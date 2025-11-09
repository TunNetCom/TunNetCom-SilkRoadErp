namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ProviderReceiptNoteLine.CreateReceiptNoteLines;

internal record class CreateReceiptNoteLigneCommand(
    List<ReceiptNoteLignes> ReceiptNoteLines
    ) : IRequest<Result<List<int>>>;

internal record class ReceiptNoteLignes(
    int RecipetNoteNumber,
    string ProductRef,
    string ProductDescription,
    int Quantity,
    decimal UnitPrice,
    double Discount,
    double Tax);
