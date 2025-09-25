namespace TunNetCom.SilkRoadErp.Sales.Contracts.RecieptNotes;

public class ReceiptNotesResponse
{
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int PageSize { get; set; }
    public List<Item> Items { get; set; }
    public int TotalCount { get; set; }
    public bool HasPrevious { get; set; }
    public bool HasNext { get; set; }
}
public class Item
{
    public int Num { get; set; }
    public int NumBonFournisseur { get; set; }
    public DateTime DateLivraison { get; set; }
    public int IdFournisseur { get; set; }
    public DateTime Date { get; set; }
    public int NumFactureFournisseur { get; set; }
}