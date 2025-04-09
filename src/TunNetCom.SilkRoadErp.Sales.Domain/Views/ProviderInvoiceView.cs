using Microsoft.EntityFrameworkCore;

namespace TunNetCom.SilkRoadErp.Sales.Domain.Views;

[Keyless]
public class ProviderInvoiceView
{
    public int Num { get; set; }

    public int ProviderId { get; set; }

    public long ProviderInvoiceNumber { get; set; }

    public DateTime InvoicingDate { get; set; }

    public DateTime Date { get; set; }

    public decimal TotalTTC { get; set; }
    public decimal TotalHT { get; set; }
    public decimal TotTva
    {
        get
        {
            return TotalTTC - TotalHT;
        }
    }
}