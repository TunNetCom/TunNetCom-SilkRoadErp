using System.Text.Json.Serialization;

namespace TunNetCom.SilkRoadErp.Sales.Contracts.RetourMarchandiseFournisseur;

/// <summary>
/// Requête pour vérifier la réception après réparation d'un retour fournisseur
/// </summary>
public class VerifyReceptionRequest
{
    /// <summary>
    /// Numéro du bon de retour fournisseur
    /// </summary>
    [JsonPropertyName("num")]
    public int Num { get; set; }

    /// <summary>
    /// Liste des lignes avec les quantités reçues
    /// </summary>
    [JsonPropertyName("lines")]
    public List<VerifyReceptionLineRequest> Lines { get; set; } = new();

    /// <summary>
    /// Commentaire optionnel sur la réception
    /// </summary>
    [JsonPropertyName("commentaire")]
    public string? Commentaire { get; set; }
}

/// <summary>
/// Détail d'une ligne de réception
/// </summary>
public class VerifyReceptionLineRequest
{
    /// <summary>
    /// Identifiant de la ligne
    /// </summary>
    [JsonPropertyName("idLigne")]
    public int IdLigne { get; set; }

    /// <summary>
    /// Quantité reçue après réparation
    /// </summary>
    [JsonPropertyName("qteRecue")]
    public int QteRecue { get; set; }
}
