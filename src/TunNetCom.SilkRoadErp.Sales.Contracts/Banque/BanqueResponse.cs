namespace TunNetCom.SilkRoadErp.Sales.Contracts.Banque;

public class BanqueResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("nom")]
    public string Nom { get; set; } = string.Empty;
}

