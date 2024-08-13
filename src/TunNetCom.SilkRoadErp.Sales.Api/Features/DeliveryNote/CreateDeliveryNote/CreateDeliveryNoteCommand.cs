using System.Text.Json.Serialization;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.CreateDeliveryNote;

public record CreateDeliveryNoteCommand(
    DateTime Date,
    decimal TotHTva,
    decimal TotTva,
    decimal NetPayer,
    TimeOnly TempBl,
    int? NumFacture,
    int? ClientId,
    IEnumerable<LigneBlRequest> Lignes)
    : IRequest<Result<int>>;

//TODO Map
public class LigneBlSubCommand
{
    [JsonPropertyName("refProduit")]
    public string RefProduit { get; set; } = string.Empty;

    [JsonPropertyName("designationLi")]
    public string DesignationLi { get; set; } = string.Empty;

    [JsonPropertyName("qteLi")]
    public int QteLi { get; set; }

    [JsonPropertyName("prixHt")]
    public decimal PrixHt { get; set; }

    [JsonPropertyName("remise")]
    public double Remise { get; set; }

    [JsonPropertyName("totHt")]
    public decimal TotHt { get; set; }

    [JsonPropertyName("tva")]
    public double Tva { get; set; }

    [JsonPropertyName("totTtc")]
    public decimal TotTtc { get; set; }
}

