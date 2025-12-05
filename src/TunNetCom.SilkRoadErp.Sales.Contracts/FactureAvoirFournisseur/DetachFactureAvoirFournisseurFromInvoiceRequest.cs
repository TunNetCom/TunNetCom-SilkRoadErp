using System.Text.Json.Serialization;

namespace TunNetCom.SilkRoadErp.Sales.Contracts.FactureAvoirFournisseur;

public class DetachFactureAvoirFournisseurFromInvoiceRequest
{
    [JsonPropertyName("factureAvoirFournisseurIds")]
    public List<int> FactureAvoirFournisseurIds { get; set; } = new();

    [JsonPropertyName("factureFournisseurId")]
    public int FactureFournisseurId { get; set; }
}


