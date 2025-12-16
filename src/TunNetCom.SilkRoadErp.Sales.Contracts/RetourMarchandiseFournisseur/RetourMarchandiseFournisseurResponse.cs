using System.Text.Json.Serialization;

namespace TunNetCom.SilkRoadErp.Sales.Contracts.RetourMarchandiseFournisseur;

public class RetourMarchandiseFournisseurResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("num")]
    public int Num { get; set; }

    [JsonPropertyName("date")]
    public DateTime Date { get; set; }

    [JsonPropertyName("idFournisseur")]
    public int IdFournisseur { get; set; }

    [JsonPropertyName("nomFournisseur")]
    public string? NomFournisseur { get; set; }

    [JsonPropertyName("totHTva")]
    public decimal TotHTva { get; set; }

    [JsonPropertyName("totTva")]
    public decimal TotTva { get; set; }

    [JsonPropertyName("netPayer")]
    public decimal NetPayer { get; set; }

    [JsonPropertyName("statut")]
    public int Statut { get; set; }

    [JsonPropertyName("statutLibelle")]
    public string StatutLibelle { get; set; } = string.Empty;

    [JsonPropertyName("lines")]
    public List<LigneRetourMarchandiseFournisseurResponse> Lines { get; set; } = new();
}

public class LigneRetourMarchandiseFournisseurResponse
{
    [JsonPropertyName("idLigne")]
    public int IdLigne { get; set; }

    [JsonPropertyName("refProduit")]
    public string RefProduit { get; set; } = string.Empty;

    [JsonPropertyName("designationLi")]
    public string DesignationLi { get; set; } = string.Empty;

    [JsonPropertyName("qteLi")]
    public int QteLi { get; set; }

    [JsonPropertyName("prixHt")]
    public decimal PrixHt { get; set; }

    [JsonPropertyName("remise")]
    public double Remise { get; set; }

    [JsonPropertyName("totHt")]
    public decimal TotHt { get; set; }

    [JsonPropertyName("tva")]
    public double Tva { get; set; }

    [JsonPropertyName("totTtc")]
    public decimal TotTtc { get; set; }

    /// <summary>
    /// Quantité déjà reçue après réparation (cumul de toutes les réceptions)
    /// </summary>
    [JsonPropertyName("qteRecue")]
    public int QteRecue { get; set; }

    /// <summary>
    /// Date de la dernière réception
    /// </summary>
    [JsonPropertyName("dateReception")]
    public DateTime? DateReception { get; set; }

    /// <summary>
    /// Quantité restant à recevoir
    /// </summary>
    [JsonPropertyName("qteRestante")]
    public int QteRestante => QteLi - QteRecue;
}

