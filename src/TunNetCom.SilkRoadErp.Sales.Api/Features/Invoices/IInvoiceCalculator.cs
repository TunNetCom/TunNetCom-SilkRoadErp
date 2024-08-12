namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Invoices;

public interface IInvoiceCalculator
{
  
    Task<decimal> CalculateTotalTTC(Facture facture);
}
