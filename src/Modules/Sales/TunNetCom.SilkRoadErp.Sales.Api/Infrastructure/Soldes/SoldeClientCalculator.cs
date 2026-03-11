namespace TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Soldes;

/// <summary>
/// Centralise la logique de calcul du solde client (formule et montant facture avec retenue/timbre)
/// pour éviter la duplication entre GetSoldeClient (détail) et SoldeClientCalculationService (bulk).
/// </summary>
public static class SoldeClientCalculator
{
    /// <summary>
    /// Montant TTC à prendre pour une facture client : retenue si présente, sinon somme des NetPayer des BL + timbre.
    /// </summary>
    /// <param name="numFacture">Numéro de la facture client.</param>
    /// <param name="montantApresRetenuByNumFacture">Dictionnaire num facture → montant après retenue (vide si pas de retenue).</param>
    /// <param name="sumNetPayerBonDeLivraison">Somme des NetPayer des bons de livraison rattachés à la facture.</param>
    /// <param name="timbre">Montant timbre fiscal.</param>
    public static decimal ComputeMontantFactureClient(
        int numFacture,
        IReadOnlyDictionary<int, decimal> montantApresRetenuByNumFacture,
        decimal sumNetPayerBonDeLivraison,
        decimal timbre)
    {
        return montantApresRetenuByNumFacture.TryGetValue(numFacture, out var montantApresRetenu)
            ? montantApresRetenu
            : sumNetPayerBonDeLivraison + timbre;
    }

    /// <summary>
    /// Formule du solde client : TotalAvoirs + TotalFacturesAvoir + TotalPaiements - TotalFactures - TotalBonsLivraisonNonFactures.
    /// </summary>
    public static decimal ComputeSolde(
        decimal totalFactures,
        decimal totalBonsLivraisonNonFactures,
        decimal totalAvoirs,
        decimal totalFacturesAvoir,
        decimal totalPaiements)
    {
        return totalAvoirs + totalFacturesAvoir + totalPaiements - totalFactures - totalBonsLivraisonNonFactures;
    }
}
