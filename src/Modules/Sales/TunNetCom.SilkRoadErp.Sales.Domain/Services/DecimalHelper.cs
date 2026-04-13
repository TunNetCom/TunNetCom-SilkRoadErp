namespace TunNetCom.SilkRoadErp.Sales.Domain.Services;

/// <summary>
/// Helper class for decimal rounding operations
/// </summary>
public static class DecimalHelper
{
    /// <summary>
    /// Default number of decimal places for amounts (prices, totals, etc.)
    /// </summary>
    private const int DEFAULT_DECIMAL_PLACES = 3;

    /// <summary>
    /// Number of decimal places for percentages (discounts, VAT rates, etc.)
    /// </summary>
    private const int PERCENTAGE_DECIMAL_PLACES = 2;

    /// <summary>
    /// Rounds a decimal amount to 3 decimal places using AwayFromZero rounding mode
    /// </summary>
    /// <param name="value">The decimal value to round</param>
    /// <returns>The rounded value with 3 decimal places</returns>
    public static decimal RoundAmount(decimal value)
    {
        return Math.Round(value, DEFAULT_DECIMAL_PLACES, MidpointRounding.AwayFromZero);
    }

    /// <summary>
    /// Rounds a decimal percentage to 2 decimal places using AwayFromZero rounding mode
    /// </summary>
    /// <param name="value">The decimal value to round</param>
    /// <returns>The rounded value with 2 decimal places</returns>
    public static decimal RoundPercentage(decimal value)
    {
        return Math.Round(value, PERCENTAGE_DECIMAL_PLACES, MidpointRounding.AwayFromZero);
    }
}

