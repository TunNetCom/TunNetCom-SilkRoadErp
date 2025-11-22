namespace TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Reports.DeliveryNotes.PrintDeliveryNote;

public class PrintDeliveryNoteModel
{
    public int Num { get; set; }
    public DateTime Date { get; set; }
    public TimeOnly DeliveryTime { get; set; }
    public int? CustomerId { get; set; }
    public int? InvoiceNumber { get; set; }
    public decimal TotalExcludingTax { get; set; }
    public decimal TotalVat { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal Timbre { get; set; }
    public decimal TotalTTC { get; set; }
    
    // TVA breakdown
    public decimal Base19 { get; set; }
    public decimal Base13 { get; set; }
    public decimal Base7 { get; set; }
    public decimal Tva19 { get; set; }
    public decimal Tva13 { get; set; }
    public decimal Tva7 { get; set; }

    public DeliveryNoteCustomerModel? Customer { get; set; }
    public List<DeliveryNoteLineModel> Lines { get; set; } = new();
}

public class DeliveryNoteCustomerModel
{
    public int Id { get; set; }
    public string Nom { get; set; } = null!;
    public string? Tel { get; set; }
    public string? Adresse { get; set; }
    public string? Matricule { get; set; }
    public string? Code { get; set; }
}

public class DeliveryNoteLineModel
{
    public int Id { get; set; }
    public string RefProduit { get; set; } = null!;
    public string DesignationLi { get; set; } = null!;
    public int QteLi { get; set; }
    public decimal PrixHt { get; set; }
    public double Remise { get; set; }
    public decimal TotHt { get; set; }
    public double Tva { get; set; }
    public decimal TotTtc { get; set; }
}

