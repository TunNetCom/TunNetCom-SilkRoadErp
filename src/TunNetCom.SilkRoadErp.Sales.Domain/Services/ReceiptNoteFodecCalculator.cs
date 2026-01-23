namespace TunNetCom.SilkRoadErp.Sales.Domain.Services;

/// <summary>
/// Centralized service for calculating FODEC for receipt note lines.
/// This ensures consistent calculation logic across all handlers.
/// Formula: FODEC = HT * taux FODEC, then TVA = (HT + FODEC) * taux TVA, then TTC = HT + FODEC + TVA
/// </summary>
public static class ReceiptNoteFodecCalculator
{
    /// <summary>
    /// Calculates FODEC and updates the TTC for a receipt note line if the provider is a constructor.
    /// </summary>
    /// <param name="totHt">Total excluding tax (HT)</param>
    /// <param name="vatRate">VAT rate percentage</param>
    /// <param name="fodecRate">FODEC rate percentage</param>
    /// <param name="isConstructor">Whether the provider is a constructor</param>
    /// <returns>A tuple containing (FODEC amount, Updated TTC)</returns>
    public static (decimal FodecAmount, decimal TotTtc) CalculateFodecAndTtc(
        decimal totHt,
        double vatRate,
        decimal fodecRate,
        bool isConstructor)
    {
        if (!isConstructor || totHt <= 0)
        {
            // For non-constructors, calculate TTC normally: HT * (1 + TVA)
            var totTtcNormal = DecimalHelper.RoundAmount(totHt * (1 + (decimal)(vatRate / 100)));
            return (0, totTtcNormal);
        }

        // Step 1: Calculate FODEC on HT
        var fodecAmount = DecimalHelper.RoundAmount(totHt * (fodecRate / 100));

        // Step 2: Calculate TVA on (HT + FODEC)
        var baseForVat = DecimalHelper.RoundAmount(totHt + fodecAmount);
        var vatAmount = DecimalHelper.RoundAmount(baseForVat * (decimal)(vatRate / 100));

        // Step 3: Calculate TTC = HT + FODEC + TVA
        var totTtcWithFodec = DecimalHelper.RoundAmount(totHt + fodecAmount + vatAmount);

        return (fodecAmount, totTtcWithFodec);
    }
}

