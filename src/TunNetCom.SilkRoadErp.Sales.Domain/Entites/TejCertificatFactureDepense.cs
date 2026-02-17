namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites;

/// <summary>
/// Attribution de la référence certificat TEJ à une facture dépense.
/// Une fois attribuée, la même ref est réutilisée pour tous les exports TEJ de cette facture.
/// </summary>
public class TejCertificatFactureDepense
{
    /// <summary>Id de la facture dépense (clé).</summary>
    public int FactureDepenseId { get; set; }

    /// <summary>Référence certificat TEJ attribuée (ex. 3001, 3002).</summary>
    public string RefCertif { get; set; } = string.Empty;

    public virtual FactureDepense? FactureDepense { get; set; }
}
