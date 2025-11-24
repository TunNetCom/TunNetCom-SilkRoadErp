using System.Text.Json.Serialization;

namespace TunNetCom.SilkRoadErp.Sales.Contracts.Inventaire;

public class LigneInventaireResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("inventaireId")]
    public int InventaireId { get; set; }

    [JsonPropertyName("refProduit")]
    public string RefProduit { get; set; } = string.Empty;

    [JsonPropertyName("nomProduit")]
    public string NomProduit { get; set; } = string.Empty;

    [JsonPropertyName("quantiteTheorique")]
    public int QuantiteTheorique { get; set; }

    [JsonPropertyName("quantiteReelle")]
    public int QuantiteReelle { get; set; }

    [JsonPropertyName("prixHt")]
    public decimal PrixHt { get; set; }

    [JsonPropertyName("dernierPrixAchat")]
    public decimal DernierPrixAchat { get; set; }
}

