using System.Text.Json.Serialization;

namespace TunNetCom.SilkRoadErp.Sales.Contracts.RetourMarchandiseFournisseur;

/// <summary>
/// Réponse après vérification de la réception
/// </summary>
public class VerifyReceptionResponse
{
    /// <summary>
    /// Indique si la réception a été enregistrée avec succès
    /// </summary>
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    /// <summary>
    /// Numéro du bon de retour
    /// </summary>
    [JsonPropertyName("num")]
    public int Num { get; set; }

    /// <summary>
    /// Nouveau statut du retour après la réception
    /// </summary>
    [JsonPropertyName("nouveauStatut")]
    public string NouveauStatut { get; set; } = string.Empty;

    /// <summary>
    /// Quantité totale reçue
    /// </summary>
    [JsonPropertyName("quantiteTotaleRecue")]
    public int QuantiteTotaleRecue { get; set; }

    /// <summary>
    /// Quantité encore en attente
    /// </summary>
    [JsonPropertyName("quantiteEnAttente")]
    public int QuantiteEnAttente { get; set; }

    /// <summary>
    /// Message d'information
    /// </summary>
    [JsonPropertyName("message")]
    public string? Message { get; set; }

    /// <summary>
    /// Détails des lignes mises à jour
    /// </summary>
    [JsonPropertyName("lignesMisesAJour")]
    public List<VerifyReceptionLineResponse> LignesMisesAJour { get; set; } = new();
}

/// <summary>
/// Détail d'une ligne après mise à jour
/// </summary>
public class VerifyReceptionLineResponse
{
    [JsonPropertyName("idLigne")]
    public int IdLigne { get; set; }

    [JsonPropertyName("refProduit")]
    public string RefProduit { get; set; } = string.Empty;

    [JsonPropertyName("designation")]
    public string Designation { get; set; } = string.Empty;

    [JsonPropertyName("qteRetournee")]
    public int QteRetournee { get; set; }

    [JsonPropertyName("qteRecue")]
    public int QteRecue { get; set; }

    [JsonPropertyName("qteEnAttente")]
    public int QteEnAttente { get; set; }
}
