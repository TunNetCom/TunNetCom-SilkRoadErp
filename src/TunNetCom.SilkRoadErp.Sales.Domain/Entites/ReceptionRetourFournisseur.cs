#nullable enable
using System;

namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites;

/// <summary>
/// Entité de traçabilité des réceptions après réparation
/// </summary>
public partial class ReceptionRetourFournisseur
{
    public static ReceptionRetourFournisseur Create(
        int retourMarchandiseFournisseurId,
        DateTime dateReception,
        string utilisateur,
        string? commentaire = null)
    {
        return new ReceptionRetourFournisseur
        {
            RetourMarchandiseFournisseurId = retourMarchandiseFournisseurId,
            DateReception = dateReception,
            Utilisateur = utilisateur,
            Commentaire = commentaire
        };
    }

    public int Id { get; set; }

    public int RetourMarchandiseFournisseurId { get; set; }

    public DateTime DateReception { get; set; }

    public string Utilisateur { get; set; } = null!;

    public string? Commentaire { get; set; }

    public virtual RetourMarchandiseFournisseur RetourMarchandiseFournisseurNavigation { get; set; } = null!;
}
