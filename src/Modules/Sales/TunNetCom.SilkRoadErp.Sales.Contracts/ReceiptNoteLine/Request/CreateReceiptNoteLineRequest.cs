namespace TunNetCom.SilkRoadErp.Sales.Contracts.ReceiptNoteLine.Request;

public record CreateReceiptNoteLineRequest(
    int RecipetNoteNumber,
    string ProductRef,
    string ProductDescription,
    int Quantity,
    decimal UnitPrice,
    double Discount,
    double Tax
);
