using TunNetCom.SilkRoadErp.Sales.Contracts.Invoice;
using TunNetCom.SilkRoadErp.Sales.Contracts;

namespace TunNetCom.SilkRoadErp.Sales.Contracts.ProviderInvoice;
public class GetProviderInvoicesWithSummary
{
    [JsonPropertyName("totalGrossAmount")]
    public decimal TotalGrossAmount { get; set; }

    [JsonPropertyName("totalVATAmount")]
    public decimal TotalVATAmount { get; set; }

    [JsonPropertyName("totalNetAmount")]
    public decimal TotalNetAmount { get; set; }

    [JsonPropertyName("invoices")]
    public PagedList<ProviderInvoiceResponse> Invoices { get; set; } = new PagedList<ProviderInvoiceResponse>();
}