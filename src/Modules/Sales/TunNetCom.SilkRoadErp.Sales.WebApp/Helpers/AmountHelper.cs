using System.Globalization;
using TunNetCom.SilkRoadErp.Sales.Contracts.Common;

namespace TunNetCom.SilkRoadErp.Sales.WebApp.Helpers;

public static class AmountHelper
{
    // Constants for decimal places - use shared constants from Contracts
    public const int DEFAULT_DECIMAL_PLACES = DecimalFormatConstants.DEFAULT_DECIMAL_PLACES;
    public const int PERCENTAGE_DECIMAL_PLACES = DecimalFormatConstants.PERCENTAGE_DECIMAL_PLACES;
    
    // Constants for format strings
    public const string FORMAT_N2 = "N2";
    public const string FORMAT_N3 = "N3";

    public const string TYPE_BL = "BL";
    public const string TYPE_DEVIS = "Devis";
    public const string TYPE_FACTURE = "Facture";
    public const string TYPE_BON_RECEPTION = "BonDeReception";
    public const string TYPE_AVOIR = "Avoir";
    public const string TYPE_FACTURE_FOURNISSEUR = "Facture Fournisseur";

    public static string FormatAmount(this decimal value)
    {
        // Default to 3 decimal places for backward compatibility
        return FormatAmount(value, DEFAULT_DECIMAL_PLACES);
    }

    public static string FormatAmount(this decimal value, int decimalPlaces)
    {
        var format = GetDecimalFormat(decimalPlaces);
        return string.Format(CultureInfo.GetCultureInfo("fr-FR"), $"{{0:{format}}}", value);
    }

    public static string GetDecimalFormat(int decimalPlaces)
    {
        // Validate that decimalPlaces is 2 or 3
        if (decimalPlaces != PERCENTAGE_DECIMAL_PLACES && decimalPlaces != DEFAULT_DECIMAL_PLACES)
        {
            decimalPlaces = DEFAULT_DECIMAL_PLACES; // Default to 3 if invalid
        }
        return $"N{decimalPlaces}";
    }

    /// <summary>
    /// Convertit un montant décimal en toutes lettres en français (dinars et millimes)
    /// </summary>
    /// <param name="montant">Le montant à convertir (en dinars avec 3 décimales pour les millimes)</param>
    /// <param name="type">Le type de document (BL, Devis, Facture, etc.)</param>
    /// <returns>Le montant en toutes lettres</returns>
    public static string ConvertFloatToFrenchToWords(decimal montant, string type)
    {
        string lettre = "";
        
        // Texte d'introduction selon le type de document
        if (type == "BL")
            lettre = "Arrêté le présent bon de livraison à la somme de ";
        else if (type == "Devis")
            lettre = "Arrêté le présent devis à la somme de ";
        else if (type == TYPE_FACTURE)
            lettre = "Arrêtée la présente facture à la somme de ";
        else if (type == "BonDeReception")
            lettre = "Arrêté le présent bon de réception à la somme de ";
        else if (type == "Avoir")
            lettre = "Arrêté le présent avoir à la somme de ";
        else if (type == "Facture Fournisseur")
            lettre = "Arrêtée la présente facture fournisseur à la somme de ";

        // Séparer la partie entière (dinars) de la partie décimale (millimes)
        int dinars = (int)montant;
        decimal partieDecimale = montant - dinars;
        int millimes = (int)Math.Round(partieDecimale * 1000, 0);

        // Convertir les dinars en lettres
        string dinarsEnLettres = ConvertNumberToWords(dinars);
        if (!string.IsNullOrEmpty(dinarsEnLettres))
        {
            lettre += dinarsEnLettres;
            if (dinars == 1)
                lettre += " dinar";
            else
                lettre += " dinars";
        }

        // Convertir les millimes en lettres
        if (millimes > 0)
        {
            if (dinars > 0)
                lettre += " et ";
            
            string millimesEnLettres = ConvertNumberToWords(millimes);
            lettre += millimesEnLettres;
            if (millimes == 1)
                lettre += " millime";
            else
                lettre += " millimes";
        }

        // Si le montant est zéro
        if (dinars == 0 && millimes == 0)
            lettre += "zéro dinars";

        return lettre;
    }

    /// <summary>
    /// Convertit un nombre entier en toutes lettres en français
    /// </summary>
    /// <param name="nombre">Le nombre à convertir (0 à 999 999 999)</param>
    /// <returns>Le nombre en toutes lettres</returns>
    private static string ConvertNumberToWords(int nombre)
    {
        if (nombre == 0)
            return "";

        string resultat = "";
        int reste = nombre;
        bool dix = false;

        // Traiter les milliards, millions, milliers, puis les unités
        for (int i = 1000000000; i >= 1; i /= 1000)
        {
            int y = reste / i;
            if (y != 0)
            {
                int centaine = y / 100;
                int dizaine = (y - (centaine * 100)) / 10;
                int unite = y - (centaine * 100) - (dizaine * 10);

                // Centaines
                switch (centaine)
                {
                    case 0:
                        break;
                    case 1:
                        resultat += "cent ";
                        break;
                    default:
                        resultat += ConvertUnitsToWords(centaine) + " cent ";
                        if (dizaine == 0 && unite == 0) resultat = resultat.TrimEnd() + "s ";
                        break;
                }

                // Dizaines et Unités
                if (dizaine == 0 && unite == 0)
                {
                    // Rien à ajouter
                }
                else if (dizaine == 0)
                {
                    resultat += ConvertUnitsToWords(unite) + " ";
                }
                else if (dizaine == 1)
                {
                    resultat += ConvertTeensToWords(unite) + " ";
                }
                else if (dizaine == 7)
                {
                    resultat += "soixante-";
                    resultat += ConvertTeensToWords(unite) + " ";
                }
                else if (dizaine == 9)
                {
                    resultat += "quatre-vingt-";
                    resultat += ConvertTeensToWords(unite) + " ";
                }
                else
                {
                    string d = ConvertDizainesToWords(dizaine);
                    if (unite == 1 && dizaine != 8)
                        resultat += d + " et un ";
                    else if (unite == 0)
                        resultat += d + (dizaine == 8 ? "s " : " ");
                    else
                        resultat += d + "-" + ConvertUnitsToWords(unite) + " ";
                }

                // Ajouter les unités de grandeur (milliards, millions, milliers, puis les unités)
                switch (i)
                {
                    case 1000000000:
                        resultat = resultat.TrimEnd() + (y > 1 ? " milliards " : " milliard ");
                        break;
                    case 1000000:
                        resultat = resultat.TrimEnd() + (y > 1 ? " millions " : " million ");
                        break;
                    case 1000:
                        if (y == 1) resultat = "mille ";
                        else resultat = resultat.TrimEnd() + " mille ";
                        break;
                }
            }

            reste -= y * i;
        }

        return resultat.Trim();
    }

    private static string ConvertUnitsToWords(int unit)
    {
        return unit switch
        {
            1 => "un",
            2 => "deux",
            3 => "trois",
            4 => "quatre",
            5 => "cinq",
            6 => "six",
            7 => "sept",
            8 => "huit",
            9 => "neuf",
            _ => ""
        };
    }

    private static string ConvertDizainesToWords(int dizaine)
    {
        return dizaine switch
        {
            2 => "vingt",
            3 => "trente",
            4 => "quarante",
            5 => "cinquante",
            6 => "soixante",
            8 => "quatre-vingt",
            _ => ""
        };
    }

    private static string ConvertTeensToWords(int unit)
    {
        return unit switch
        {
            0 => "dix",
            1 => "onze",
            2 => "douze",
            3 => "treize",
            4 => "quatorze",
            5 => "quinze",
            6 => "seize",
            7 => "dix-sept",
            8 => "dix-huit",
            9 => "dix-neuf",
            _ => ""
        };
    }
}
