using System.Globalization;

namespace TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Soldes;

/// <summary>
/// Centralise la logique de calcul du solde fournisseur (formule et montant facture avec retenue/timbre)
/// pour éviter la duplication entre GetSoldeFournisseur (détail) et SoldeFournisseurCalculationService (bulk).
/// Formule alignée sur le popup "Gérer facture" : ((facture - timbre) - avoir financier - factures avoirs) × (1 - taux retenue) + timbre.
/// </summary>
public static class SoldeFournisseurCalculator
{
    /// <summary>
    /// Montant TTC à prendre pour une facture fournisseur : retenue si présente (déjà nette d'avoirs),
    /// sinon (somme des lignes BR + timbre - avoirs rattachés à cette facture). Les avoirs ne sont ainsi soustraits qu'une fois.
    /// </summary>
    /// <param name="numFacture">Numéro de la facture fournisseur.</param>
    /// <param name="montantApresRetenuByNumFacture">Dictionnaire num facture → montant après retenue (déjà net d'avoirs).</param>
    /// <param name="sumLigneBonReceptionTtc">Somme des TotTtc des lignes de bon de réception rattachées.</param>
    /// <param name="timbre">Montant timbre fiscal.</param>
    /// <param name="avoirsForThisFacture">Somme des avoirs (facture avoir + avoirs financiers) rattachés à cette facture (pour les factures sans retenue).</param>
    public static decimal ComputeMontantFactureFournisseur(
        int numFacture,
        IReadOnlyDictionary<int, decimal> montantApresRetenuByNumFacture,
        decimal sumLigneBonReceptionTtc,
        decimal timbre,
        decimal avoirsForThisFacture = 0m)
    {
        if (montantApresRetenuByNumFacture.TryGetValue(numFacture, out var montantApresRetenu))
            return montantApresRetenu;
        var net = sumLigneBonReceptionTtc + timbre - avoirsForThisFacture;
        return net < 0 ? 0 : net;
    }

    /// <summary>
    /// Calcule le montant facture fournisseur avec la formule du popup "Gérer facture" :
    /// ((BR - timbre) - avoir financier - factures avoirs) × (1 - taux retenue) + timbre si retenue, sinon (BR + timbre - avoirs) plafonné à 0.
    /// Retourne le montant et une chaîne décrivant la formule pour affichage.
    /// </summary>
    /// <param name="sumLigneBonReceptionTtc">Somme des TotTtc des lignes BR (facture hors timbre).</param>
    /// <param name="timbre">Montant timbre fiscal.</param>
    /// <param name="avoirsFinanciers">Somme des avoirs financiers rattachés à la facture.</param>
    /// <param name="facturesAvoir">Somme des factures avoir rattachées à la facture.</param>
    /// <param name="tauxRetenuPourcent">Taux de retenue à la source (en %) si retenue présente, sinon null.</param>
    /// <returns>Montant final et formule textuelle.</returns>
    public static (decimal Montant, string Formule) ComputeMontantEtFormuleFactureFournisseur(
        decimal sumLigneBonReceptionTtc,
        decimal timbre,
        decimal avoirsFinanciers,
        decimal facturesAvoir,
        double? tauxRetenuPourcent)
    {
        var totalAvoirs = avoirsFinanciers + facturesAvoir;
        var n2 = CultureInfo.CurrentCulture.NumberFormat;
        var brStr = sumLigneBonReceptionTtc.ToString("N2", n2);
        var timbreStr = timbre.ToString("N2", n2);
        var afStr = avoirsFinanciers.ToString("N2", n2);
        var faStr = facturesAvoir.ToString("N2", n2);

        if (tauxRetenuPourcent.HasValue && tauxRetenuPourcent.Value >= 0)
        {
            // Base = (Facture - Timbre) - avoirs = BR - avoirs (aligné popup Gérer facture)
            var baseRetenue = sumLigneBonReceptionTtc - totalAvoirs;
            if (baseRetenue < 0) baseRetenue = 0;
            var montantApresRetenuSansTimbre = baseRetenue * (1 - (decimal)tauxRetenuPourcent.Value / 100);
            var montant = montantApresRetenuSansTimbre + timbre;
            if (montant < 0) montant = 0;
            var factureStr = (sumLigneBonReceptionTtc + timbre).ToString("N2", n2); // Facture = BR + timbre
            var tauxStr = tauxRetenuPourcent.Value.ToString("N1", n2);
            var resultStr = montant.ToString("N2", n2);
            var formule = $"(({factureStr} - {timbreStr}) - {afStr} - {faStr}) × (1 - {tauxStr} %) + {timbreStr} = {resultStr}";
            return (montant, formule);
        }

        var net = sumLigneBonReceptionTtc + timbre - totalAvoirs;
        var montantSansRetenue = net < 0 ? 0 : net;
        var formuleSansRetenue = totalAvoirs > 0
            ? $"({brStr} + {timbreStr}) - {afStr} - {faStr} = {montantSansRetenue.ToString("N2", n2)}"
            : $"{brStr} + {timbreStr} = {montantSansRetenue.ToString("N2", n2)}";
        return (montantSansRetenue, formuleSansRetenue);
    }

    /// <summary>
    /// Formule du solde fournisseur : TotalFactures (brut) + TotalBRNonFacturés - TotalFacturesAvoir - TotalAvoirsFinanciers - TotalPaiements.
    /// Utilisée quand les factures sont affichées en brut (BR+timbre) ; les avoirs sont soustraits une seule fois ici.
    /// </summary>
    public static decimal ComputeSolde(
        decimal totalFactures,
        decimal totalBonsReceptionNonFactures,
        decimal totalFacturesAvoir,
        decimal totalAvoirsFinanciers,
        decimal totalPaiements)
    {
        return totalFactures + totalBonsReceptionNonFactures - totalFacturesAvoir - totalAvoirsFinanciers - totalPaiements;
    }

    /// <summary>
    /// Solde lorsque les factures sont en montant après retenue (avoirs déjà déduits par facture) :
    /// Somme(montants après retenue par facture) + TotalBRNonFacturés - TotalPaiements.
    /// </summary>
    public static decimal ComputeSoldeAvecRetenue(
        decimal totalMontantsApresRetenueParFacture,
        decimal totalBonsReceptionNonFactures,
        decimal totalPaiements)
    {
        return totalMontantsApresRetenueParFacture + totalBonsReceptionNonFactures - totalPaiements;
    }
}
