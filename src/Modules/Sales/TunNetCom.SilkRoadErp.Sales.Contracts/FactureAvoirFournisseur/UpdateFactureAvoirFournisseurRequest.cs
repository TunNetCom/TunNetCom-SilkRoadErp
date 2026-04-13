namespace TunNetCom.SilkRoadErp.Sales.Contracts.FactureAvoirFournisseur;

public class UpdateFactureAvoirFournisseurRequest
{
    [JsonPropertyName("date")]
    public DateTime Date { get; set; }

    [JsonPropertyName("idFournisseur")]
    public int IdFournisseur { get; set; }

    [JsonPropertyName("numFactureAvoirFourSurPage")]
    public int NumFactureAvoirFourSurPage { get; set; }

    [JsonPropertyName("numFactureFournisseur")]
    public int? NumFactureFournisseur { get; set; }

    [JsonPropertyName("avoirFournisseurIds")]
    public List<int> AvoirFournisseurIds { get; set; } = new();
}

