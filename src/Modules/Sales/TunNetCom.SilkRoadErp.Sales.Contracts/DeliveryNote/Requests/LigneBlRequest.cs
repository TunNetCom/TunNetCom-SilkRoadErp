namespace TunNetCom.SilkRoadErp.Sales.Contracts.DeliveryNote.Requests;

public class LigneBlRequest
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("refProduit")]
    public string RefProduit { get; set; } = string.Empty;

    [JsonPropertyName("designationLi")]
    public string DesignationLi { get; set; } = string.Empty;

    [JsonPropertyName("qteLi")]
    public int QteLi { get; set; }

    [JsonPropertyName("qteLivree")]
    public int? QteLivree { get; set; }

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
