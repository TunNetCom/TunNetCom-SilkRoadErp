namespace TunNetCom.SilkRoadErp.Sales.Contracts.Invoice;

public class GetInvoicesWithSummariesResponse
{
    [JsonPropertyName("totalNetAmount")]
    public decimal TotalNetAmount { get; set; }

    [JsonPropertyName("totalVatAmount")]
    public decimal TotalVatAmount { get; set; }

    [JsonPropertyName("invoices")]
    public PagedList<InvoiceBaseInfo> Invoices { get; set; } = new PagedList<InvoiceBaseInfo>();
}

public class InvoiceBaseInfo
{
    [JsonPropertyName("number")]
    public int Number { get; set; }

    [JsonPropertyName("date")]
    public DateTimeOffset Date { get; set; }

    [JsonPropertyName("customerId")]
    public int CustomerId { get; set; }

    [JsonPropertyName("customerName")]
    public string? CustomerName { get; set; }

    [JsonPropertyName("customerCode")]
    public string? CustomerCode { get; set; }

    [JsonPropertyName("netAmount")]
    public decimal NetAmount { get; set; }

    [JsonPropertyName("vatAmount")]
    public decimal VatAmount { get; set; }

    [JsonPropertyName("statut")]
    public int Statut { get; set; }

    [JsonPropertyName("statutLibelle")]
    public string StatutLibelle { get; set; } = string.Empty;
}

