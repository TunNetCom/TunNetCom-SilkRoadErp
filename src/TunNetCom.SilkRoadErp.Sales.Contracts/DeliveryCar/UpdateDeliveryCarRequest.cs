using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TunNetCom.SilkRoadErp.Sales.Contracts.DeliveryCar;

public class UpdateDeliveryCarRequest
{
    [Required]
    [MaxLength(50)]
    [JsonPropertyName("matricule")]
    public string Matricule { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    [JsonPropertyName("owner")]
    public string Owner { get; set; } = string.Empty;
}

