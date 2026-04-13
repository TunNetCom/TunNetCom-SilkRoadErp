namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites;

/// <summary>
/// Statuts spécifiques pour les retours marchandise fournisseur
/// </summary>
public enum RetourFournisseurStatus
{
    /// <summary>
    /// Brouillon - Le retour est en cours de création
    /// </summary>
    Draft = 0,
    
    /// <summary>
    /// Validé - Le retour a été envoyé au fournisseur
    /// </summary>
    Valid = 1,
    
    /// <summary>
    /// En réparation - Les produits sont chez le fournisseur pour réparation
    /// </summary>
    EnReparation = 2,
    
    /// <summary>
    /// Réception partielle - Une partie des produits a été reçue après réparation
    /// </summary>
    ReceptionPartielle = 3,
    
    /// <summary>
    /// Clôturé - Tous les produits ont été reçus après réparation
    /// </summary>
    Cloture = 4
}
