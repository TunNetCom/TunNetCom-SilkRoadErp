namespace TunNetCom.SilkRoadErp.Sales.Contracts.RecieptNotes;

public class ReceiptNoteDetailsResponse
{
    [JsonPropertyName("num")]
    public int Num { get; set; }

    [JsonPropertyName("NumBonFournisseur")]
    public long NumBonFournisseur { get; set; }

    [JsonPropertyName("DateLivraison")]
    public DateTime DateLivraison { get; set; }

    [JsonPropertyName("IdFournisseur")]
    public int IdFournisseur { get; set; }

    [JsonPropertyName("Date")]
    public DateTime Date { get; set; }

    [JsonPropertyName("NumFactureFournisseur")]
    public int? NumFactureFournisseur { get; set; }

    [JsonPropertyName("totTTC")]
    public decimal TotTTC { get; set; }

    [JsonPropertyName("totHTva")]
    public decimal TotHTva { get; set; }

    [JsonPropertyName("totTva")]
    public decimal TotTva { get; set; }
}
