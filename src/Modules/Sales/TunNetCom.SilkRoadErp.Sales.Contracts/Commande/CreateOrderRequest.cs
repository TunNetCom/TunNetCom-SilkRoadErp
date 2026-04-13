using System.Text.Json.Serialization;

namespace TunNetCom.SilkRoadErp.Sales.Contracts.Commande;

public class CreateOrderRequest
{
    [JsonPropertyName("fournisseurId")]
    public int? FournisseurId { get; set; }

    [JsonPropertyName("date")]
    public DateTime Date { get; set; }

    [JsonPropertyName("totHTva")]
    public decimal TotHTva { get; set; }

    [JsonPropertyName("TotTva")]
    public decimal TotTva { get; set; }

    [JsonPropertyName("TotTtc")]
    public decimal TotTtc { get; set; }

    [JsonPropertyName("items")]
    public List<OrderItemRequest> Items { get; set; } = new();
}

