namespace TunNetCom.SilkRoadErp.Sales.Contracts.Common;

/// <summary>
/// Default VAT rate values used only when current accounting year parameters are not available (fallback).
/// Normal behaviour is to resolve rates from the current accounting year (AppParameters / GetAppParametersResponse).
/// </summary>
public static class VatRateConstants
{
    /// <summary>Fallback VAT rate 0% when accounting year is not loaded.</summary>
    public const double DefaultVatRate0 = 0;

    /// <summary>Fallback VAT rate 7% when accounting year is not loaded.</summary>
    public const double DefaultVatRate7 = 7;

    /// <summary>Fallback VAT rate 13% when accounting year is not loaded.</summary>
    public const double DefaultVatRate13 = 13;

    /// <summary>Fallback VAT rate 19% when accounting year is not loaded (also used as default when article rate is not 7 or 19).</summary>
    public const double DefaultVatRate19 = 19;
}
