using System.Text.Json.Serialization;

namespace TunNetCom.SilkRoadErp.Sales.Contracts.RetourMarchandiseFournisseur;

public class CreateRetourMarchandiseFournisseurRequest
{
    [JsonPropertyName("date")]
    public DateTime Date { get; set; }

    [JsonPropertyName("idFournisseur")]
    public int IdFournisseur { get; set; }

    [JsonPropertyName("lines")]
    public List<RetourMarchandiseFournisseurLineRequest> Lines { get; set; } = new();
}

public class RetourMarchandiseFournisseurLineRequest
{
    [JsonPropertyName("productRef")]
    public string ProductRef { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }

    [JsonPropertyName("unitPrice")]
    public decimal UnitPrice { get; set; }

    [JsonPropertyName("discount")]
    public double Discount { get; set; }

    [JsonPropertyName("tax")]
    public double Tax { get; set; }

    /// <summary>
    /// Quantité reçue après réparation (optionnel, pour réception partielle)
    /// </summary>
    [JsonPropertyName("qteRecue")]
    public int? QteRecue { get; set; }
}

