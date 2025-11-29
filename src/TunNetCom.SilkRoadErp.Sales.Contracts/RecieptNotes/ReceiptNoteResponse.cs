using TunNetCom.SilkRoadErp.Sales.Contracts.ReceiptNote.Responses;

namespace TunNetCom.SilkRoadErp.Sales.Contracts.RecieptNotes;

public class ReceiptNoteResponse
{
    [JsonPropertyName("num")]
    public int Num { get; set; }

    [JsonPropertyName("NumBonFournisseur")]
    public long NumBonFournisseur { get; set; }

    [JsonPropertyName("DateLivraison")]
    public DateTime DateLivraison { get; set; }

    [JsonPropertyName("IdFournisseur")]
    public int IdFournisseur { get; set; }

    public string NomFournisseur { get; set; }

    [JsonPropertyName("Date")]
    public DateTime Date { get; set; }

    [JsonPropertyName("NumFactureFournisseur")]
    public int? NumFactureFournisseur { get; set; }

    [JsonPropertyName("statut")]
    public int Statut { get; set; }

    [JsonPropertyName("statutLibelle")]
    public string StatutLibelle { get; set; } = string.Empty;

    [JsonPropertyName("items")]
    public List<ReceiptNoteDetailResponse> Items { get; set; } = new();
}