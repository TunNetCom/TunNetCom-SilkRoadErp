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

    public static string ConvertFloatToFrenchToWords(float chiffre, string type)
    {
        int centaine, dizaine, unite, reste, y;
        bool dix = false;
        string lettre = "";
        if (type == "BL")
            lettre = "Arrêté le present bon de livraison à la somme de ";
        if (type == "Devis")
            lettre = "Arrêté le present devis à la somme de ";
        if (type == TYPE_FACTURE)
            lettre = "Arrêté la présente facture à la somme  de ";
        if (type == "BonDeReception")
            lettre = "Arrêté le present bon de réception à la somme de ";
        if (type == "Avoir")
            lettre = "Arrêté le present Avoir à la somme de ";

        if (type == "Facture Fournisseur")
            lettre = "Arrêté la présente facture fournisseur à la somme de ";
        //strcpy(lettre, "");

        reste = (int)chiffre / 1;

        for (int i = 1000000000; i >= 1; i /= 1000)
        {
            y = reste / i;
            if (y != 0)
            {
                centaine = y / 100;
                dizaine = (y - (centaine * 100)) / 10;
                unite = y - (centaine * 100) - (dizaine * 10);
                switch (centaine)
                {
                    case 0:
                        break;
                    case 1:
                        lettre += "cent ";
                        break;
                    case 2:
                        if ((dizaine == 0) && (unite == 0)) lettre += "deux cents ";
                        else lettre += "deux cent ";
                        break;
                    case 3:
                        if ((dizaine == 0) && (unite == 0)) lettre += "trois cents ";
                        else lettre += "trois cent ";
                        break;
                    case 4:
                        if ((dizaine == 0) && (unite == 0)) lettre += "quatre cents ";
                        else lettre += "quatre cent ";
                        break;
                    case 5:
                        if ((dizaine == 0) && (unite == 0)) lettre += "cinq cents ";
                        else lettre += "cinq cent ";
                        break;
                    case 6:
                        if ((dizaine == 0) && (unite == 0)) lettre += "six cents ";
                        else lettre += "six cent ";
                        break;
                    case 7:
                        if ((dizaine == 0) && (unite == 0)) lettre += "sept cents ";
                        else lettre += "sept cent ";
                        break;
                    case 8:
                        if ((dizaine == 0) && (unite == 0)) lettre += "huit cents ";
                        else lettre += "huit cent ";
                        break;
                    case 9:
                        if ((dizaine == 0) && (unite == 0)) lettre += "neuf cents ";
                        else lettre += "neuf cent ";
                        break;
                }// endSwitch(centaine)

                switch (dizaine)
                {
                    case 0:
                        break;
                    case 1:
                        dix = true;
                        break;
                    case 2:
                        lettre += "vingt ";
                        break;
                    case 3:
                        lettre += "trente ";
                        break;
                    case 4:
                        lettre += "quarante ";
                        break;
                    case 5:
                        lettre += "cinquante ";
                        break;
                    case 6:
                        lettre += "soixante ";
                        break;
                    case 7:
                        dix = true;
                        lettre += "soixante ";
                        break;
                    case 8:
                        lettre += "quatre-vingt ";
                        break;
                    case 9:
                        dix = true;
                        lettre += "quatre-vingt ";
                        break;
                } // endSwitch(dizaine)

                switch (unite)
                {
                    case 0:
                        if (dix) lettre += "dix ";
                        break;
                    case 1:
                        if (dix) lettre += "onze ";
                        else lettre += "un ";
                        break;
                    case 2:
                        if (dix) lettre += "douze ";
                        else lettre += "deux ";
                        break;
                    case 3:
                        if (dix) lettre += "treize ";
                        else lettre += "trois ";
                        break;
                    case 4:
                        if (dix) lettre += "quatorze ";
                        else lettre += "quatre ";
                        break;
                    case 5:
                        if (dix) lettre += "quinze ";
                        else lettre += "cinq ";
                        break;
                    case 6:
                        if (dix) lettre += "seize ";
                        else lettre += "six ";
                        break;
                    case 7:
                        if (dix) lettre += "dix-sept ";
                        else lettre += "sept ";
                        break;
                    case 8:
                        if (dix) lettre += "dix-huit ";
                        else lettre += "huit ";
                        break;
                    case 9:
                        if (dix) lettre += "dix-neuf ";
                        else lettre += "neuf ";
                        break;
                } // endSwitch(unite)

                switch (i)
                {
                    case 1000000000:
                        if (y > 1) lettre += "millions ";
                        else lettre += "million ";
                        break;
                    case 1000000:
                        if (y > 1) lettre += "milles ";
                        else lettre += "mille ";
                        break;
                    case 1000:
                        lettre += "dinars et ";
                        break;
                    case 1:
                        lettre += "millimes ";
                        break;
                }
            } // end if(y!=0)
            reste -= y * i;
            dix = false;
        } // end for
        if (lettre.Length == 0) lettre += "zero";


        return lettre;
    }
}
