using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TunNetCom.SilkRoadErp.Sales.Contracts.AccountingYear;

public class UpdateAccountingYearRequest
{
    [Required]
    [JsonPropertyName("year")]
    public int Year { get; set; }

    [Required]
    [JsonPropertyName("isActive")]
    public bool IsActive { get; set; }
}
