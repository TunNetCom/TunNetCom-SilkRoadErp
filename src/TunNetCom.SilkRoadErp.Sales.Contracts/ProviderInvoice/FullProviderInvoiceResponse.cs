namespace TunNetCom.SilkRoadErp.Sales.Contracts.ProviderInvoice;

public class FullProviderInvoiceResponse
{
    public int ProviderInvoiceNumber { get; set; }
    public DateTime Date { get; set; }
    public int ProviderId { get; set; }

    public ProviderInfos Provider { get; set; } = null!;
    public List<FullProviderInvoiceReceiptNotes> ReceiptNotes { get; set; } = new();
}

public class ProviderInfos
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string? Fax { get; set; }

    public string? RegistrationNumber { get; set; }

    public string? Code { get; set; }

    public string? CategoryCode { get; set; }

    public string? SecondaryEstablishment { get; set; }

    public string? Mail { get; set; }

    public string? Adress { get; set; }
}

public class FullProviderInvoiceReceiptNotes
{
    public int ReceiptNoteNumber { get; set; }

    public DateTime Date { get; set; }

    public decimal TotalExcludingVat { get; set; }

    public decimal TotalVat { get; set; }

    public decimal NetToPay { get; set; }

    public TimeOnly DeliveryTime { get; set; }

    public int? ProviderId { get; set; }

    public List<ReceiptNotesLine> Lines { get; set; } = new();
}

public class ReceiptNotesLine
{
    public int LineId { get; set; }
    public string ProductReference { get; set; } = null!;
    public string ItemDescription { get; set; } = null!;
    public int ItemQuantity { get; set; }
    public decimal UnitPriceExcludingTax { get; set; }
    public double Discount { get; set; }
    public decimal TotalExcludingTax { get; set; }
    public double VatRate { get; set; }
    public decimal TotalIncludingTax { get; set; }
}
