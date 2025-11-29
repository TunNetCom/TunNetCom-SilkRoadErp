namespace TunNetCom.SilkRoadErp.Sales.Contracts.FactureAvoirFournisseur;

public class FactureAvoirFournisseurResponse
{
    [JsonPropertyName("num")]
    public int Num { get; set; }

    [JsonPropertyName("numFactureAvoirFourSurPage")]
    public int NumFactureAvoirFourSurPage { get; set; }

    [JsonPropertyName("date")]
    public DateTime Date { get; set; }

    [JsonPropertyName("idFournisseur")]
    public int IdFournisseur { get; set; }

    [JsonPropertyName("numFactureFournisseur")]
    public int? NumFactureFournisseur { get; set; }

    [JsonPropertyName("statut")]
    public int Statut { get; set; }

    [JsonPropertyName("statutLibelle")]
    public string StatutLibelle { get; set; } = string.Empty;
}

