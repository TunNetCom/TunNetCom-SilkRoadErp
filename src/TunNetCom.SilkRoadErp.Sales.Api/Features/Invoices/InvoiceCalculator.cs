namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Invoices;

public class InvoiceCalculator(SalesContext _context) : IInvoiceCalculator
{
    public async Task<decimal> CalculateTotalTTC(Facture facture)
    {
        var timbre = await _context.Systeme.Select(s => s.Timbre).FirstOrDefaultAsync();
        return facture.BonDeLivraison.Sum(b => b.NetPayer) + timbre;
    }

    public async Task<decimal> CalculateTotalHt(Facture facture)
    {
        if (facture?.BonDeLivraison == null)
            return 0;

        return await Task.FromResult(facture.BonDeLivraison.Sum(b => b.TotHTva));
    }

    public async Task<decimal> CalculateTotalTTva(Facture facture)
    {
        if (facture?.BonDeLivraison == null)
            return 0;

        return await Task.FromResult(facture.BonDeLivraison.Sum(b => b.TotTva));
    }
}
