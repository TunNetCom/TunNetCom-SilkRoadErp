namespace TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Soldes;

/// <summary>
/// Centralise la logique de calcul du solde fournisseur (formule et montant facture avec retenue/timbre)
/// pour éviter la duplication entre GetSoldeFournisseur (détail) et SoldeFournisseurCalculationService (bulk).
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
}
