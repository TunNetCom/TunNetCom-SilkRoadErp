using TunNetCom.SilkRoadErp.Sales.Contracts.Common;

namespace TunNetCom.SilkRoadErp.Sales.WebApp.Helpers;

/// <summary>
/// Generic calculator for line items (delivery notes, invoices, etc.)
/// </summary>
public static class LineItemCalculator
{
    /// <summary>
    /// Calculates the totals for a single line item
    /// </summary>
    /// <typeparam name="T">The type of line item that implements ILineItem</typeparam>
    /// <param name="lineItem">The line item to calculate totals for</param>
    public static void CalculateTotals<T>(T lineItem) where T : ILineItem
    {
        if (lineItem.Quantity > 0 && lineItem.UnitPriceExcludingTax > 0)
        {
            decimal totalBeforeDiscount = lineItem.Quantity * lineItem.UnitPriceExcludingTax;
            decimal discountAmount = totalBeforeDiscount * (decimal)(lineItem.DiscountPercentage / 100);
            lineItem.TotalExcludingTax = totalBeforeDiscount - discountAmount;
            decimal vatAmount = lineItem.TotalExcludingTax * (decimal)(lineItem.VatPercentage / 100);
            lineItem.TotalIncludingTax = lineItem.TotalExcludingTax + vatAmount;
        }
        else
        {
            lineItem.TotalExcludingTax = 0;
            lineItem.TotalIncludingTax = 0;
        }
    }

    /// <summary>
    /// Calculates and returns the aggregated totals for a collection of line items
    /// </summary>
    /// <typeparam name="T">The type of line item that implements ILineItem</typeparam>
    /// <param name="lineItems">The collection of line items</param>
    /// <returns>A LineItemTotals object containing the aggregated totals</returns>
    public static LineItemTotals CalculateAggregatedTotals<T>(IEnumerable<T> lineItems) where T : ILineItem
    {
        var itemsList = lineItems.ToList();
        
        return new LineItemTotals
        {
            TotalHt = itemsList.Sum(o => o.TotalExcludingTax),
            TotalVat = itemsList.Sum(o => o.TotalIncludingTax - o.TotalExcludingTax),
            TotalTtc = itemsList.Sum(o => o.TotalIncludingTax)
        };
    }

    /// <summary>
    /// Updates the totals for a collection of line items and returns the aggregated totals
    /// </summary>
    /// <typeparam name="T">The type of line item that implements ILineItem</typeparam>
    /// <param name="lineItems">The collection of line items</param>
    /// <param name="totalHt">Output parameter for total excluding tax</param>
    /// <param name="totalVat">Output parameter for total VAT</param>
    /// <param name="totalTtc">Output parameter for total including tax</param>
    public static void UpdateTotals<T>(
        IEnumerable<T> lineItems,
        out decimal totalHt,
        out decimal totalVat,
        out decimal totalTtc) where T : ILineItem
    {
        var totals = CalculateAggregatedTotals(lineItems);
        totalHt = totals.TotalHt;
        totalVat = totals.TotalVat;
        totalTtc = totals.TotalTtc;
    }
}

/// <summary>
/// Represents the aggregated totals for a collection of line items
/// </summary>
public class LineItemTotals
{
    public decimal TotalHt { get; set; }
    public decimal TotalVat { get; set; }
    public decimal TotalTtc { get; set; }
}
