using System.Text.Json.Serialization;

namespace TunNetCom.SilkRoadErp.Sales.Contracts.BankTransaction;

public class BankTransactionImportResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("compteBancaireId")]
    public int CompteBancaireId { get; set; }

    [JsonPropertyName("compteBancaireLibelle")]
    public string? CompteBancaireLibelle { get; set; }

    [JsonPropertyName("fileName")]
    public string FileName { get; set; } = string.Empty;

    [JsonPropertyName("importedAt")]
    public DateTime ImportedAt { get; set; }

    [JsonPropertyName("rowCount")]
    public int RowCount { get; set; }
}
