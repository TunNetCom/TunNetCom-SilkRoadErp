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
                    case 2:
                        if ((dizaine == 0) && (unite == 0)) resultat += "deux cents ";
                        else resultat += "deux cent ";
                        break;
                    case 3:
                        if ((dizaine == 0) && (unite == 0)) resultat += "trois cents ";
                        else resultat += "trois cent ";
                        break;
                    case 4:
                        if ((dizaine == 0) && (unite == 0)) resultat += "quatre cents ";
                        else resultat += "quatre cent ";
                        break;
                    case 5:
                        if ((dizaine == 0) && (unite == 0)) resultat += "cinq cents ";
                        else resultat += "cinq cent ";
                        break;
                    case 6:
                        if ((dizaine == 0) && (unite == 0)) resultat += "six cents ";
                        else resultat += "six cent ";
                        break;
                    case 7:
                        if ((dizaine == 0) && (unite == 0)) resultat += "sept cents ";
                        else resultat += "sept cent ";
                        break;
                    case 8:
                        if ((dizaine == 0) && (unite == 0)) resultat += "huit cents ";
                        else resultat += "huit cent ";
                        break;
                    case 9:
                        if ((dizaine == 0) && (unite == 0)) resultat += "neuf cents ";
                        else resultat += "neuf cent ";
                        break;
                }

                // Dizaines
                dix = false;
                switch (dizaine)
                {
                    case 0:
                        break;
                    case 1:
                        dix = true;
                        break;
                    case 2:
                        resultat += "vingt ";
                        break;
                    case 3:
                        resultat += "trente ";
                        break;
                    case 4:
                        resultat += "quarante ";
                        break;
                    case 5:
                        resultat += "cinquante ";
                        break;
                    case 6:
                        resultat += "soixante ";
                        break;
                    case 7:
                        dix = true;
                        resultat += "soixante ";
                        break;
                    case 8:
                        resultat += "quatre-vingt ";
                        break;
                    case 9:
                        dix = true;
                        resultat += "quatre-vingt ";
                        break;
                }

                // Unités
                switch (unite)
                {
                    case 0:
                        if (dix) resultat += "dix ";
                        break;
                    case 1:
                        if (dix) resultat += "onze ";
                        else resultat += "un ";
                        break;
                    case 2:
                        if (dix) resultat += "douze ";
                        else resultat += "deux ";
                        break;
                    case 3:
                        if (dix) resultat += "treize ";
                        else resultat += "trois ";
                        break;
                    case 4:
                        if (dix) resultat += "quatorze ";
                        else resultat += "quatre ";
                        break;
                    case 5:
                        if (dix) resultat += "quinze ";
                        else resultat += "cinq ";
                        break;
                    case 6:
                        if (dix) resultat += "seize ";
                        else resultat += "six ";
                        break;
                    case 7:
                        if (dix) resultat += "dix-sept ";
                        else resultat += "sept ";
                        break;
                    case 8:
                        if (dix) resultat += "dix-huit ";
                        else resultat += "huit ";
                        break;
                    case 9:
                        if (dix) resultat += "dix-neuf ";
                        else resultat += "neuf ";
                        break;
                }

                // Ajouter les unités de grandeur (milliards, millions, mille)
                switch (i)
                {
                    case 1000000000:
                        if (y > 1) resultat += "milliards ";
                        else resultat += "milliard ";
                        break;
                    case 1000000:
                        if (y > 1) resultat += "millions ";
                        else resultat += "million ";
                        break;
                    case 1000:
                        resultat += "mille ";
                        break;
                }
            }

            reste -= y * i;
        }

        return resultat;
    }
}
