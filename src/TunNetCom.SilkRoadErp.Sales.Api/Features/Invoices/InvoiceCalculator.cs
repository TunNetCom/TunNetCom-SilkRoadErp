
using Microsoft.EntityFrameworkCore;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Invoices;

public class InvoiceCalculator(SalesContext _context) : IInvoiceCalculator
{
    public decimal CalculateTotalHTva(Facture facture)
    {
        return facture.BonDeLivraison.Sum(b => b.TotHTva);
    }

    public decimal CalculateTotalTva(Facture facture)
    {
        return facture.BonDeLivraison.Sum(b => b.TotTva);
    }

    public async Task<decimal> CalculateTotalTTC(Facture facture)
    {
        var timbre = await _context.Systeme.Select(s => s.Timbre).FirstOrDefaultAsync();
        return facture.BonDeLivraison.Sum(b => b.NetPayer) + timbre;
    }
}
