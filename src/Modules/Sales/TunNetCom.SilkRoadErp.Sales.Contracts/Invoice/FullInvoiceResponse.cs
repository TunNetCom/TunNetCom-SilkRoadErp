namespace TunNetCom.SilkRoadErp.Sales.Contracts.Invoice;

public class FullInvoiceResponse
{
    public int Num { get; set; }
    public DateTime Date { get; set; }
    public int IdClient { get; set; }
    public int Statut { get; set; }
    public string StatutLibelle { get; set; } = string.Empty;

    public FullInvoiceCustomerResponse Client { get; set; } = null!;
    public List<FullInvoiceCustomerResponseDeliveryNoteResponse> DeliveryNotes { get; set; } = new();
}

public class FullInvoiceCustomerResponse
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

public class FullInvoiceCustomerResponseDeliveryNoteResponse
{
    public int Num { get; set; }
    public DateTime Date { get; set; }
    public decimal TotHTva { get; set; }
    public decimal TotTva { get; set; }
    public decimal NetPayer { get; set; }
    public TimeOnly TempBl { get; set; }
    public int? ClientId { get; set; }

    public List<DeliveryNoteLineResponse> Lines { get; set; } = new();
}

public class DeliveryNoteLineResponse
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
