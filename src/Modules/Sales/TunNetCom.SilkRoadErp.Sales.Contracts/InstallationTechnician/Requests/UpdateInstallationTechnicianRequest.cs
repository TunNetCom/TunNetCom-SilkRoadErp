namespace TunNetCom.SilkRoadErp.Sales.Contracts.InstallationTechnician.Requests;

public class UpdateInstallationTechnicianRequest
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("nom")]
    public string Nom { get; set; } = string.Empty;

    [JsonPropertyName("tel")]
    public string? Tel { get; set; }

    [JsonPropertyName("tel2")]
    public string? Tel2 { get; set; }

    [JsonPropertyName("tel3")]
    public string? Tel3 { get; set; }

    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("photo")]
    public string? Photo { get; set; }
}

