using System.Text.Json.Serialization;

namespace TunNetCom.SilkRoadErp.Sales.Contracts.BankTransaction;

public class ImportBankTransactionsResponse
{
    [JsonPropertyName("importId")]
    public int ImportId { get; set; }

    [JsonPropertyName("rowsImported")]
    public int RowsImported { get; set; }

    [JsonPropertyName("fileName")]
    public string FileName { get; set; } = string.Empty;

    [JsonPropertyName("errors")]
    public IReadOnlyList<string> Errors { get; set; } = Array.Empty<string>();
}
