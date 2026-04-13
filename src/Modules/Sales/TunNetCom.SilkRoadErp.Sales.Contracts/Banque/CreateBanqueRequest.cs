namespace TunNetCom.SilkRoadErp.Sales.Contracts.Banque;

public class CreateBanqueRequest
{
    [JsonPropertyName("nom")]
    public string Nom { get; set; } = string.Empty;
}

