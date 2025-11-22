using Microsoft.EntityFrameworkCore;

namespace TunNetCom.SilkRoadErp.Sales.Domain.Views;

[Keyless]
public class ReceiptNoteView
{
    public int Num { get; set; }

    public long NumBonFournisseur { get; set; }

    public DateTime DateLivraison { get; set; }

    public int IdFournisseur { get; set; }

    public DateTime Date { get; set; }

    public int? NumFactureFournisseur { get; set; }

    public decimal TotalTTC { get; set; }

    public decimal TotHt { get; set; }

    public decimal TotTva 
    {
        get
        {
            return TotalTTC - TotHt;
        }
    }
}
