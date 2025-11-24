#nullable enable
using System;

namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites;

public partial class LigneInventaire
{
    private LigneInventaire()
    {
    }

    public static LigneInventaire CreateLigneInventaire(
        int inventaireId,
        string refProduit,
        int quantiteTheorique,
        int quantiteReelle,
        decimal prixHt,
        decimal dernierPrixAchat)
    {
        return new LigneInventaire
        {
            InventaireId = inventaireId,
            RefProduit = refProduit,
            QuantiteTheorique = quantiteTheorique,
            QuantiteReelle = quantiteReelle,
            PrixHt = prixHt,
            DernierPrixAchat = dernierPrixAchat
        };
    }

    public void UpdateLigneInventaire(
        int quantiteReelle,
        decimal prixHt,
        decimal dernierPrixAchat)
    {
        QuantiteReelle = quantiteReelle;
        PrixHt = prixHt;
        DernierPrixAchat = dernierPrixAchat;
    }

    public int Id { get; private set; }

    public int InventaireId { get; private set; }

    public string RefProduit { get; private set; } = null!;

    public int QuantiteTheorique { get; private set; }

    public int QuantiteReelle { get; private set; }

    public decimal PrixHt { get; private set; }

    public decimal DernierPrixAchat { get; private set; }

    public virtual Inventaire Inventaire { get; set; } = null!;

    public virtual Produit RefProduitNavigation { get; set; } = null!;
}

