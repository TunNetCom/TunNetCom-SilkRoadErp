namespace TunNetCom.SilkRoadErp.Sales.Contracts.Invoice;

public class GetInvoiceListWithSummary
{
    [JsonPropertyName("totalGrossAmount")]
    public decimal TotalGrossAmount { get; set; }

    [JsonPropertyName("totalVATAmount")]
    public decimal TotalVATAmount { get; set; }

    [JsonPropertyName("totalNetAmount")]
    public decimal TotalNetAmount { get; set; }

    [JsonPropertyName("invoices")]
    public PagedList<InvoiceResponse> Invoices { get; set; } = new PagedList<InvoiceResponse>();
}
