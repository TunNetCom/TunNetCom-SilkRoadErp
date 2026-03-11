namespace TunNetCom.SilkRoadErp.Sales.Domain.Services;

/// <summary>
/// Shared aggregator for HT/TVA totals by rate (7, 13, 19).
/// Used by invoice totals, provider invoice totals, and recap endpoints to avoid duplicating calculation logic.
/// </summary>
public static class VatTotalsAggregator
{
    /// <summary>
    /// Aggregates line totals by VAT rate and optionally adds timbre to HT (e.g. one per invoice).
    /// </summary>
    /// <param name="lines">Lines with TotHt, TotTtc, and Tva (int rate, e.g. 7, 13, 19).</param>
    /// <param name="vatRate7">VAT rate value for 7% (e.g. 7).</param>
    /// <param name="vatRate13">VAT rate value for 13% (e.g. 13).</param>
    /// <param name="vatRate19">VAT rate value for 19% (e.g. 19).</param>
    /// <param name="timbre">Stamp tax to add to total HT (e.g. one per document).</param>
    /// <param name="documentCount">Number of documents to apply timbre to (e.g. invoice count). Use 0 to skip timbre.</param>
    /// <returns>Aggregated totals (HT, bases and VAT by rate, total VAT, TTC).</returns>
    public static VatTotalsResult Aggregate(
        IEnumerable<VatLineItem> lines,
        int vatRate7,
        int vatRate13,
        int vatRate19,
        decimal timbre = 0,
        int documentCount = 0)
    {
        var list = lines.ToList();

        var totalHT = list.Sum(l => l.TotHt);
        var totalBase7 = list.Where(l => l.Tva == vatRate7).Sum(l => l.TotHt);
        var totalBase13 = list.Where(l => l.Tva == vatRate13).Sum(l => l.TotHt);
        var totalBase19 = list.Where(l => l.Tva == vatRate19).Sum(l => l.TotHt);
        var totalVat7 = list.Where(l => l.Tva == vatRate7).Sum(l => l.TotTtc - l.TotHt);
        var totalVat13 = list.Where(l => l.Tva == vatRate13).Sum(l => l.TotTtc - l.TotHt);
        var totalVat19 = list.Where(l => l.Tva == vatRate19).Sum(l => l.TotTtc - l.TotHt);
        var totalVat = totalVat7 + totalVat13 + totalVat19;

        if (documentCount > 0 && timbre > 0)
        {
            totalHT += timbre * documentCount;
        }

        var totalTTC = totalHT + totalVat;

        return new VatTotalsResult
        {
            TotalHT = totalHT,
            TotalBase7 = totalBase7,
            TotalBase13 = totalBase13,
            TotalBase19 = totalBase19,
            TotalVat7 = totalVat7,
            TotalVat13 = totalVat13,
            TotalVat19 = totalVat19,
            TotalVat = totalVat,
            TotalTTC = totalTTC
        };
    }
}

/// <summary>
/// A single line item for VAT aggregation (HT, TTC, and VAT rate as int).
/// </summary>
public readonly record struct VatLineItem(decimal TotHt, decimal TotTtc, int Tva);

/// <summary>
/// Result of VAT totals aggregation (neutral DTO in Domain; API maps to InvoiceTotalsResponse / ProviderInvoiceTotalsResponse).
/// </summary>
public class VatTotalsResult
{
    public decimal TotalHT { get; set; }
    public decimal TotalBase7 { get; set; }
    public decimal TotalBase13 { get; set; }
    public decimal TotalBase19 { get; set; }
    public decimal TotalVat7 { get; set; }
    public decimal TotalVat13 { get; set; }
    public decimal TotalVat19 { get; set; }
    public decimal TotalVat { get; set; }
    public decimal TotalTTC { get; set; }
}
