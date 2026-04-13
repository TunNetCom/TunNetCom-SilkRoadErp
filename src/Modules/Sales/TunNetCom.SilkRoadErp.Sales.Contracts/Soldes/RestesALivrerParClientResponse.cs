using System.Text.Json.Serialization;

namespace TunNetCom.SilkRoadErp.Sales.Contracts.Soldes;

public class RestesALivrerParClientResponse
{
    [JsonPropertyName("clients")]
    public List<ClientRestesALivrerItem> Clients { get; set; } = new();
}

public class ClientRestesALivrerItem
{
    [JsonPropertyName("clientId")]
    public int ClientId { get; set; }

    [JsonPropertyName("clientNom")]
    public string ClientNom { get; set; } = string.Empty;

    [JsonPropertyName("solde")]
    public decimal Solde { get; set; }

    [JsonPropertyName("lignesRestesALivrer")]
    public List<LigneResteaLivrer> LignesRestesALivrer { get; set; } = new();
}

public class LigneResteaLivrer
{
    [JsonPropertyName("refProduit")]
    public string RefProduit { get; set; } = string.Empty;

    [JsonPropertyName("designationLi")]
    public string DesignationLi { get; set; } = string.Empty;

    [JsonPropertyName("quantiteRestante")]
    public int QuantiteRestante { get; set; }
}
