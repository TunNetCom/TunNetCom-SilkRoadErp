namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Invoices;

public interface IInvoiceCalculator
{
    decimal CalculateTotalHTva(Facture facture);
    decimal CalculateTotalTva(Facture facture);
    Task<decimal> CalculateTotalTTC(Facture facture);
}
