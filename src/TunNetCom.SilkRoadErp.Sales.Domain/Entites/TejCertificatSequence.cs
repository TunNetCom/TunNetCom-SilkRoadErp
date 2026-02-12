using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites;

/// <summary>
/// Séquence mensuelle pour la référence certificat TEJ (Ref_certif_chez_declarant).
/// Format généré : {Mois}{XXX} (ex. mars → 3001, 3002 ; janvier → 1001, 1002).
/// La séquence est réinitialisée chaque mois.
/// </summary>
public class TejCertificatSequence
{
    public int Annee { get; set; }
    public int Mois { get; set; }
    public int DerniereSequence { get; set; }

    [Timestamp]
    public byte[]? RowVersion { get; set; }
}
