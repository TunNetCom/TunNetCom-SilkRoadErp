namespace TunNetCom.SilkRoadErp.Sales.Contracts.RecieptNotes;

public class CreateReceiptNoteRequest
{
    public int Num { get; set; }

    public long NumBonFournisseur { get; set; }

    public DateTime DateLivraison { get; set; }

    public int IdFournisseur { get; set; }

    public DateTime Date { get; set; }

    public int? NumFactureFournisseur { get; set; }
}
