namespace TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Reports.Invoices.ProviderInvoices;

public class ProviderInvoiceModel
{
    public int InvoiceNumber { get; set; }
    public DateTime IssueDate { get; set; }
    public int ProviderId { get; set; }

    public ProviderDetailsModel Provider { get; set; } = null!;
    public List<ProviderReceiptNoteModel> ReceiptNotes { get; set; } = new(); // Added for receipt notes
    public decimal TotalHT { get; internal set; }
    public decimal TotalTVA { get; internal set; }
    public decimal TotalTTC { get; internal set; }
    public decimal Base19 { get; internal set; }
    public decimal Base7 { get; internal set; }
    public decimal Tva19 { get; internal set; }
    public decimal Tva7 { get; internal set; }
    public decimal Base13 { get; internal set; }
    public decimal Tva13 { get; internal set; }
    public decimal Timbre { get; set; }
    public decimal VatRate0 { get; set; }
    public decimal VatRate7 { get; set; }
    public decimal VatRate13 { get; set; }
    public decimal VatRate19 { get; set; }

}

public class ProviderDetailsModel
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? TaxId { get; set; }
    public string? ProviderCode { get; set; }
    public string? CategoryCode { get; set; }
    public string? SecondaryEstablishment { get; set; }
    public string? Email { get; set; }
}

public class ProviderReceiptNoteModel
{
    public int ReceiptNoteId { get; set; }
    public DateTime ReceiptDate { get; set; }
    public string? ReceiverName { get; set; } // Name of person/entity receiving goods
    public string? Status { get; set; } // e.g., "Received", "Partial", "Discrepancy"
    public int ReceivedQuantity { get; set; } // Total quantity received
    public decimal ReceivedValueExclTax { get; set; } // Value of received goods (HT)
    public decimal ReceivedTaxAmount { get; set; } // Tax on received goods
    public decimal ReceivedValueInclTax { get; set; } // Total value (TTC)
    public string? Comments { get; set; } // Notes on discrepancies or issues
    public int? ProviderId { get; set; }

    public List<ProviderReceiptLineModel> Lines { get; set; } = new(); // Details of received items
}

public class ProviderReceiptLineModel
{
    public int LineId { get; set; }
    public string ProductCode { get; set; } = null!; // Matches ProviderNoteLineModel.ProductCode
    public string Description { get; set; } = null!;
    public int OrderedQuantity { get; set; } // Quantity per delivery note
    public int ReceivedQuantity { get; set; } // Quantity actually received
    public decimal UnitPriceExclTax { get; set; }
    public double Remise { get; set; }
    public double TaxRate { get; set; }
    public decimal LineTotalExclTax { get; set; } // Received value (HT)
    public decimal LineTotalInclTax { get; set; } // Received value (TTC)
}