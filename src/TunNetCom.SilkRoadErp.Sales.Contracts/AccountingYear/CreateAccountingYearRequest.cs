using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TunNetCom.SilkRoadErp.Sales.Contracts.AccountingYear;

public class CreateAccountingYearRequest
{
    [Required]
    [JsonPropertyName("year")]
    public int Year { get; set; }

    [JsonPropertyName("isActive")]
    public bool IsActive { get; set; } = false;
}
