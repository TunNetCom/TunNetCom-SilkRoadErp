namespace TunNetCom.SilkRoadErp.Sales.Contracts.Soldes;

public class ClientSoldeProblemeResponse
{
    [JsonPropertyName("clientId")]
    public int ClientId { get; set; }

    [JsonPropertyName("clientNom")]
    public string ClientNom { get; set; } = string.Empty;

    [JsonPropertyName("solde")]
    public decimal Solde { get; set; }

    [JsonPropertyName("nombreQuantitesNonLivrees")]
    public int NombreQuantitesNonLivrees { get; set; }

    [JsonPropertyName("totalFactures")]
    public decimal TotalFactures { get; set; }

    [JsonPropertyName("totalPaiements")]
    public decimal TotalPaiements { get; set; }

    [JsonPropertyName("dateDernierDocument")]
    public DateTime? DateDernierDocument { get; set; }
}

