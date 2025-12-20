using System.Text.Json.Serialization;

namespace TunNetCom.SilkRoadErp.Sales.Contracts.AccountingYear;

public class AccountingYearResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("year")]
    public int Year { get; set; }

    [JsonPropertyName("isActive")]
    public bool IsActive { get; set; }
}
