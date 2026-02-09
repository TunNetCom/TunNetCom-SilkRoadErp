using System.Text.Json.Serialization;

namespace TunNetCom.SilkRoadErp.Sales.Contracts.CompteBancaire;

public class CreateCompteBancaireRequest
{
    [JsonPropertyName("banqueId")]
    public int BanqueId { get; set; }

    [JsonPropertyName("codeEtablissement")]
    public string CodeEtablissement { get; set; } = string.Empty;

    [JsonPropertyName("codeAgence")]
    public string CodeAgence { get; set; } = string.Empty;

    [JsonPropertyName("numeroCompte")]
    public string NumeroCompte { get; set; } = string.Empty;

    [JsonPropertyName("cleRib")]
    public string CleRib { get; set; } = string.Empty;

    [JsonPropertyName("libelle")]
    public string? Libelle { get; set; }
}
