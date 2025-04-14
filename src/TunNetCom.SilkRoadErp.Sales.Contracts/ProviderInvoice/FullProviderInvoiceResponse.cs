using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TunNetCom.SilkRoadErp.Sales.Contracts.ProviderInvoice;

public class FullProviderInvoiceResponse
{
    public int Num { get; set; }
    public DateTime Date { get; set; }
    public int ProviderId { get; set; }

    public ProviderInfos Provider { get; set; } = null!;
    public List<FullProviderInvoiceReceiptNotes> ReceiptNotes { get; set; } = new();
}

public class ProviderInfos
{
    public int Id { get; set; }

    public string Nom { get; set; } = null!;

    public string Tel { get; set; } = null!;

    public string? Fax { get; set; }

    public string? Matricule { get; set; }

    public string? Code { get; set; }

    public string? CodeCat { get; set; }

    public string? EtbSec { get; set; }

    public string? Mail { get; set; }

    public string? Adresse { get; set; }
}

public class FullProviderInvoiceReceiptNotes
{
    public int Num { get; set; }

    public DateTime Date { get; set; }

    public decimal TotHTva { get; set; }

    public decimal TotTva { get; set; }

    public decimal NetPayer { get; set; }

    public TimeOnly TempBl { get; set; }

    public int? ProviderId { get; set; }

    public List<ReceiptNotesLine> Lines { get; set; } = new();
}

public class ReceiptNotesLine
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
