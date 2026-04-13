using System.Text.Json.Serialization;

namespace TunNetCom.SilkRoadErp.Sales.Contracts.Tej;

public class ExportAllToTejXmlRequest
{
    [JsonPropertyName("providerInvoiceNumbers")]
    public List<int> ProviderInvoiceNumbers { get; set; } = new();

    [JsonPropertyName("factureDepenseIds")]
    public List<int> FactureDepenseIds { get; set; } = new();

    /// <summary>Acte de dépôt : "0" (déclaration initiale) ou "1" (rectificative).</summary>
    [JsonPropertyName("acteDepot")]
    public string ActeDepot { get; set; } = "0";

    /// <summary>Année de dépôt pour ReferenceDeclaration (ex. 2025).</summary>
    [JsonPropertyName("anneeDepot")]
    public int AnneeDepot { get; set; }

    /// <summary>Mois de dépôt pour ReferenceDeclaration (1-12).</summary>
    [JsonPropertyName("moisDepot")]
    public int MoisDepot { get; set; }
}
