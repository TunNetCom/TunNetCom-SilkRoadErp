namespace TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Reports.Invoices.ProviderInvoices;

public class ProviderInvoiceModel
{
    public int InvoiceNumber { get; set; }
    public DateTime IssueDate { get; set; }
    public int ProviderId { get; set; }

    public ProviderDetailsModel Provider { get; set; } = null!;
    public List<ProviderReceiptNoteModel> ReceiptNotes { get; set; } = new(); // Added for receipt notes
    public decimal SubtotalExclTax { get; internal set; } // HT: Hors Taxes (excl. tax)
    public decimal TotalTax { get; internal set; } // TVA: Tax amount
    public decimal TotalInclTax { get; internal set; } // TTC: Toutes Taxes Comprises (incl. tax)
    public decimal TaxBase19 { get; internal set; } // Base for 19% tax rate
    public decimal TaxBase7 { get; internal set; } // Base for 7% tax rate
    public decimal TaxAmount19 { get; internal set; } // Tax at 19%
    public decimal TaxAmount7 { get; internal set; } // Tax at 7%
    public decimal TaxBase13 { get; internal set; } // Base for 13% tax rate
    public decimal TaxAmount13 { get; internal set; } // Tax at 13%
    public decimal StampDuty { get; set; } // Timbre: Stamp or fiscal duty
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
    public int DeliveryNoteNumber { get; set; } // Links to ProviderDeliveryNoteModel.NoteNumber
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
    public double TaxRate { get; set; }
    public decimal LineTotalExclTax { get; set; } // Received value (HT)
    public decimal LineTotalInclTax { get; set; } // Received value (TTC)
    public string? DiscrepancyNote { get; set; } // Notes on quantity/value mismatches
}