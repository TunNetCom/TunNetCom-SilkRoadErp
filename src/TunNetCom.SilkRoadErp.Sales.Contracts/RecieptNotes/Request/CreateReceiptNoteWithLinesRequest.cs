namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ReceiptNote.CreateReceiptNote;

public class CreateReceiptNoteWithLinesRequest
{
    public long NumBonFournisseur { get; set; }

    public DateTime DateLivraison { get; set; }

    public int IdFournisseur { get; set; }

    public DateTime Date { get; set; }

    public int? NumFactureFournisseur { get; set; }

    public List<ReceiptNoteLineRequest> ReceiptNoteLines { get; set; } = new();
}

public record ReceiptNoteLineRequest(
    string ProductRef,
    string ProductDescription,
    int Quantity,
    decimal UnitPrice,
    double Discount,
    double Tax
);