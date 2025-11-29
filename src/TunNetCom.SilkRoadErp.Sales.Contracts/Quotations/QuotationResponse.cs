using System.Text.Json.Serialization;

namespace TunNetCom.SilkRoadErp.Sales.Contracts.Quotations
{
    public class QuotationResponse
    {
        [JsonPropertyName("num")]
        public int Num { get; set; }

        [JsonPropertyName("idClient")]
        public int IdClient { get; set; }

        [JsonPropertyName("customerName")]
        public string CustomerName { get; set; } = string.Empty;

        [JsonPropertyName("date")]
        public DateTime Date { get; set; }

        [JsonPropertyName("totHTva")]
        public decimal TotHTva { get; set; }

        [JsonPropertyName("TotTva")]
        public decimal TotTva { get; set; }

        [JsonPropertyName("TotTtc")]
        public decimal TotTtc { get; set; }

        [JsonPropertyName("statut")]
        public int Statut { get; set; }

        [JsonPropertyName("statutLibelle")]
        public string StatutLibelle { get; set; } = string.Empty;
    }
}
