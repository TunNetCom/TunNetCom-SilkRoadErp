using System.Globalization;

namespace TunNetCom.SilkRoadErp.Sales.WebApp.Helpers;

public static class AmountHelper
{
    public static string FormatNumber(this decimal value)
    {
        // Convert to string with US formatting, ensuring 3 decimal places
        string numberString = value.ToString("N3", CultureInfo.GetCultureInfo("en-US"));

        // Split into integer and decimal parts
        string[] parts = numberString.Split('.');
        string integerPart = parts[0];
        string decimalPart = parts[1].TrimEnd('0');

        // If decimal part is empty after trimming, use "0"
        return decimalPart.Length == 0
            ? $"{integerPart}.0"
            : $"{integerPart}.{decimalPart}";
    }
}
