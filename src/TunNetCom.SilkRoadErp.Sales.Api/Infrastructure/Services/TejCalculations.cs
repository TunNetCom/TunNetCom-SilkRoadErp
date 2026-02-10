namespace TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services;

/// <summary>
/// Centralise tous les calculs liés à l'export TEJ (retenue à la source).
/// Formules CCT : montants en millimes ; la plateforme TEJ effectue le calcul de retenue à partir du montant avant retenue et du taux.
/// </summary>
public static class TejCalculations
{
    /// <summary>
    /// Convertit un montant en dinars en millimes (1 dinar = 1000 millimes), arrondi à 3 décimales.
    /// </summary>
    public static long ToMillimesLong(decimal amountInDinars)
    {
        var rounded = Math.Round(amountInDinars, 3, MidpointRounding.AwayFromZero);
        return (long)(rounded * 1000);
    }

    /// <summary>
    /// Calcule le montant TVA en millimes : MontantHT × TauxTVA / 100 (formule CCT).
    /// </summary>
    public static long ComputeMontantTvaMillimes(long montantHTMillimes, decimal tauxTvaPercent)
    {
        return (long)Math.Round(montantHTMillimes * (double)tauxTvaPercent / 100, MidpointRounding.AwayFromZero);
    }

    /// <summary>
    /// Calcule le montant retenue à la source en millimes : MontantTTC × TauxRS / 100 (formule CCT).
    /// La plateforme TEJ utilise cette formule ; on l'expose pour cohérence du XML si requis par le schéma.
    /// </summary>
    public static long ComputeMontantRsMillimes(long montantTTCMillimes, decimal tauxRsPercent)
    {
        return (long)Math.Round(montantTTCMillimes * (double)tauxRsPercent / 100, MidpointRounding.AwayFromZero);
    }

    /// <summary>
    /// Calcule le montant net servi en millimes : MontantTTC - MontantRS.
    /// </summary>
    public static long ComputeMontantNetServiMillimes(long montantTTCMillimes, long montantRSMillimes)
    {
        return montantTTCMillimes - montantRSMillimes;
    }

    /// <summary>
    /// À partir du montant TTC avant retenue (en millimes) et du taux TVA, déduit le montant HT en millimes
    /// pour que MontantTVA = MontantHT × TauxTVA/100 et MontantHT + MontantTVA = MontantTTC.
    /// </summary>
    public static (long MontantHTMillimes, long MontantTVAMillimes) DeriveHtAndTvaFromTtc(long montantTTCMillimes, decimal tauxTvaPercent)
    {
        if (tauxTvaPercent <= 0)
        {
            return (montantTTCMillimes, 0);
        }
        // MontantTTC = MontantHT * (1 + TauxTVA/100) => MontantHT = MontantTTC / (1 + TauxTVA/100)
        var montantHTMillimes = (long)Math.Round(montantTTCMillimes / (1 + (double)tauxTvaPercent / 100), MidpointRounding.AwayFromZero);
        var montantTVAMillimes = montantTTCMillimes - montantHTMillimes; // pour que HT + TVA = TTC exactement
        return (montantHTMillimes, montantTVAMillimes);
    }
}
