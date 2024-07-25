using System.Text.Json.Serialization;

namespace TunNetCom.SilkRoadErp.Sales.Contracts.DeliveryNote;

public class LigneBlRequest
{
    [JsonPropertyName("refProduit")]
    public string RefProduit { get; set; } = null!;

    [JsonPropertyName("designationLi")]
    public string DesignationLi { get; set; } = null!;

    [JsonPropertyName("qteLi")]
    public int QteLi { get; set; }

    [JsonPropertyName("prixHt")]
    public decimal PrixHt { get; set; }

    [JsonPropertyName("remise")]
    public double Remise { get; set; }

    [JsonPropertyName("totHt")]
    public decimal TotHt { get; set; }

    [JsonPropertyName("tva")]
    public double Tva { get; set; }

    [JsonPropertyName("totTtc")]
    public decimal TotTtc { get; set; }
}
