using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TunNetCom.SilkRoadErp.Sales.Contracts.ProviderInvoice;

public class ProviderInvoiceResponse
{
    [JsonPropertyName("num")]
    public int Num { get; set; }

    [JsonPropertyName("providerId")]
    public int ProviderId { get; set; }

    [JsonPropertyName("date")]
    public DateTime Date { get; set; }

    [JsonPropertyName("dateFacturation")]
    public DateTime DateFacturation { get; set; }

    [JsonPropertyName("totTTC")]
    public decimal TotTTC { get; set; }

    [JsonPropertyName("totHTva")]
    public decimal TotHTva { get; set; }

    [JsonPropertyName("totTva")]
    public decimal TotTva { get; set; }
}
