namespace TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Soldes;

/// <summary>
/// Centralise la logique de calcul du solde fournisseur (formule et montant facture avec retenue/timbre)
/// pour éviter la duplication entre GetSoldeFournisseur (détail) et SoldeFournisseurCalculationService (bulk).
/// </summary>
public static class SoldeFournisseurCalculator
{
    /// <summary>
    /// Montant TTC à prendre pour une facture fournisseur : retenue si présente, sinon somme des lignes BR + timbre.
    /// </summary>
    /// <param name="numFacture">Numéro de la facture fournisseur.</param>
    /// <param name="montantApresRetenuByNumFacture">Dictionnaire num facture → montant après retenue (vide si pas de retenue).</param>
    /// <param name="sumLigneBonReceptionTtc">Somme des TotTtc des lignes de bon de réception rattachées.</param>
    /// <param name="timbre">Montant timbre fiscal.</param>
    public static decimal ComputeMontantFactureFournisseur(
        int numFacture,
        IReadOnlyDictionary<int, decimal> montantApresRetenuByNumFacture,
        decimal sumLigneBonReceptionTtc,
        decimal timbre)
    {
        return montantApresRetenuByNumFacture.TryGetValue(numFacture, out var montantApresRetenu)
            ? montantApresRetenu
            : sumLigneBonReceptionTtc + timbre;
    }

    /// <summary>
    /// Formule du solde fournisseur : TotalFactures + TotalBRNonFacturés - TotalFacturesAvoir - TotalAvoirsFinanciers - TotalPaiements.
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
