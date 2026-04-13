namespace TunNetCom.SilkRoadErp.Sales.Contracts.Invoice;

public class GetInvoiceListWithSummary
{
    [JsonPropertyName("totalExcludingTaxAmount")]
    public decimal TotalExcludingTaxAmount { get; set; }

    [JsonPropertyName("totalVATAmount")]
    public decimal TotalVATAmount { get; set; }

    [JsonPropertyName("TotalIncludingTaxAmount")]
    public decimal TotalIncludingTaxAmount { get; set; }

    [JsonPropertyName("invoices")]
    public PagedList<InvoiceResponse> Invoices { get; set; } = new PagedList<InvoiceResponse>();
}
