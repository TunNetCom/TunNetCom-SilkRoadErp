namespace TunNetCom.SilkRoadErp.Sales.Contracts.AvoirFinancierFournisseurs;

public class ValidateAvoirFinancierFournisseursRequest
{
    [JsonPropertyName("ids")]
    public List<int> Ids { get; set; } = new();
}

