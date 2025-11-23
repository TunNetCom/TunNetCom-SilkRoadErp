namespace TunNetCom.SilkRoadErp.Sales.Contracts.AvoirFournisseur;

public class UpdateAvoirFournisseurRequest
{
    [JsonPropertyName("date")]
    public DateTime Date { get; set; }

    [JsonPropertyName("fournisseurId")]
    public int? FournisseurId { get; set; }

    [JsonPropertyName("numFactureAvoirFournisseur")]
    public int? NumFactureAvoirFournisseur { get; set; }

    [JsonPropertyName("lines")]
    public List<AvoirFournisseurLineRequest> Lines { get; set; } = new();
}

