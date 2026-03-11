namespace TunNetCom.SilkRoadErp.Sales.Contracts.InstallationTechnician.Responses;

public class InstallationTechnicianBaseInfo
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("nom")]
    public string Nom { get; set; } = string.Empty;

    [JsonPropertyName("photo")]
    public string? Photo { get; set; }
}

