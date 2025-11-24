using System;
using System.Text.Json.Serialization;

namespace TunNetCom.SilkRoadErp.Sales.Contracts.Inventaire;

public class HistoriqueAchatVenteResponse
{
    [JsonPropertyName("date")]
    public DateTime Date { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty; // "Achat" ou "Vente"

    [JsonPropertyName("documentNum")]
    public int DocumentNum { get; set; }

    [JsonPropertyName("fournisseurClient")]
    public string? FournisseurClient { get; set; }

    [JsonPropertyName("quantite")]
    public int Quantite { get; set; }

    [JsonPropertyName("prixHt")]
    public decimal PrixHt { get; set; }

    [JsonPropertyName("totalHt")]
    public decimal TotalHt { get; set; }
}

