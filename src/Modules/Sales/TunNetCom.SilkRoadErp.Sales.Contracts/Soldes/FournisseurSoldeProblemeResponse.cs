namespace TunNetCom.SilkRoadErp.Sales.Contracts.Soldes;

public class FournisseurSoldeProblemeResponse
{
    [JsonPropertyName("fournisseurId")]
    public int FournisseurId { get; set; }

    [JsonPropertyName("fournisseurNom")]
    public string FournisseurNom { get; set; } = string.Empty;

    [JsonPropertyName("solde")]
    public decimal Solde { get; set; }

    [JsonPropertyName("totalFactures")]
    public decimal TotalFactures { get; set; }

    [JsonPropertyName("totalFacturesAvoir")]
    public decimal TotalFacturesAvoir { get; set; }

    [JsonPropertyName("totalAvoirsFinanciers")]
    public decimal TotalAvoirsFinanciers { get; set; }

    [JsonPropertyName("totalPaiements")]
    public decimal TotalPaiements { get; set; }

    [JsonPropertyName("dateDernierDocument")]
    public DateTime? DateDernierDocument { get; set; }
}
