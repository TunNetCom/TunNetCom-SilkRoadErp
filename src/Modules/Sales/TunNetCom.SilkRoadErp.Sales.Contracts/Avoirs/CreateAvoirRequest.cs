namespace TunNetCom.SilkRoadErp.Sales.Contracts.Avoirs;

public class CreateAvoirRequest
{
    [JsonPropertyName("date")]
    public DateTime Date { get; set; }

    [JsonPropertyName("clientId")]
    public int? ClientId { get; set; }

    [JsonPropertyName("lines")]
    public List<AvoirLineRequest> Lines { get; set; } = new();
}

public class AvoirLineRequest
{
    [JsonPropertyName("refProduit")]
    public string RefProduit { get; set; } = string.Empty;

    [JsonPropertyName("designationLi")]
    public string? DesignationLi { get; set; }

    [JsonPropertyName("qteLi")]
    public int QteLi { get; set; }

    [JsonPropertyName("prixHt")]
    public decimal PrixHt { get; set; }

    [JsonPropertyName("remise")]
    public double Remise { get; set; }

    [JsonPropertyName("tva")]
    public double Tva { get; set; }
}

