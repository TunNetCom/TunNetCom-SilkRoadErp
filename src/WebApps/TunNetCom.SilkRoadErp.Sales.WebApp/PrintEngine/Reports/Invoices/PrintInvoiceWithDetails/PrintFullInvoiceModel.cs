namespace TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Reports.Invoices.PrintInvoiceWithDetails;

public class PrintFullInvoiceModel
{
    public int Num { get; set; }
    public DateTime Date { get; set; }
    public int IdClient { get; set; }

    public FullInvoiceCustomerModel Client { get; set; } = null!;
    public List<FullInvoiceCustomerResponseDeliveryNoteModel> DeliveryNotes { get; set; } = new();
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

public class FullInvoiceCustomerModel
{
    public int Id { get; set; }
    public string Nom { get; set; } = null!;
    public string? Tel { get; set; }
    public string? Adresse { get; set; }
    public string? Matricule { get; set; }
    public string? Code { get; set; }
    public string? CodeCat { get; set; }
    public string? EtbSec { get; set; }
    public string? Mail { get; set; }
}

public class FullInvoiceCustomerResponseDeliveryNoteModel
{
    public int Num { get; set; }
    public DateTime Date { get; set; }
    public decimal TotHTva { get; set; }
    public decimal TotTva { get; set; }
    public decimal NetPayer { get; set; }
    public TimeOnly TempBl { get; set; }
    public int? ClientId { get; set; }

    public List<DeliveryNoteLineModel> Lines { get; set; } = new();
}

public class DeliveryNoteLineModel
{
    public int IdLi { get; set; }
    public string RefProduit { get; set; } = null!;
    public string DesignationLi { get; set; } = null!;
    public int QteLi { get; set; }
    public decimal PrixHt { get; set; }
    public double Remise { get; set; }
    public decimal TotHt { get; set; }
    public double Tva { get; set; }
    public decimal TotTtc { get; set; }
}

