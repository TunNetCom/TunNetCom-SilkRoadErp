#nullable enable
using System;
using System.Collections.Generic;
using TunNetCom.SilkRoadErp.Sales.Domain.Services;

namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites;

public partial class LigneRetourMarchandiseFournisseur
{
    public static LigneRetourMarchandiseFournisseur CreateRetourLine(
        int retourMarchandiseFournisseurId,
        string productRef,
        string designationLigne,
        int quantity,
        decimal unitPrice,
        double discount,
        double tax,
        int? qteRecue = null)
    {
        var subtotalBeforeDiscount = DecimalHelper.RoundAmount(quantity * unitPrice);
        var discountAmount = DecimalHelper.RoundAmount(subtotalBeforeDiscount * (decimal)(discount / 100));
        var subtotalAfterDiscount = DecimalHelper.RoundAmount(subtotalBeforeDiscount - discountAmount);
        var totTtc = DecimalHelper.RoundAmount(subtotalAfterDiscount * (1 + (decimal)(tax / 100)));
        
        return new LigneRetourMarchandiseFournisseur
        {
            RefProduit = productRef,
            DesignationLi = designationLigne,
            QteLi = quantity,
            PrixHt = unitPrice,
            Remise = discount,
            TotHt = subtotalAfterDiscount,
            Tva = tax,
            TotTtc = totTtc,
            RetourMarchandiseFournisseurId = retourMarchandiseFournisseurId,
            QteRecue = qteRecue ?? 0
        };
    }

    /// <summary>
    /// Met à jour la quantité reçue après réparation
    /// </summary>
    /// <param name="quantiteRecue">La quantité reçue</param>
    /// <param name="utilisateur">L'utilisateur ayant validé la réception</param>
    public void EnregistrerReception(int quantiteRecue, string utilisateur)
    {
        if (quantiteRecue < 0)
        {
            throw new ArgumentException("La quantité reçue ne peut pas être négative.", nameof(quantiteRecue));
        }
        
        if (quantiteRecue > QteLi)
        {
            throw new ArgumentException($"La quantité reçue ({quantiteRecue}) ne peut pas dépasser la quantité retournée ({QteLi}).", nameof(quantiteRecue));
        }

        QteRecue = quantiteRecue;
        DateReception = DateTime.Now;
        UtilisateurReception = utilisateur;
    }

    /// <summary>
    /// Calcule la quantité encore en attente de réception
    /// </summary>
    public int GetQuantiteEnAttente()
    {
        return Math.Max(0, QteLi - QteRecue);
    }

    /// <summary>
    /// Indique si la ligne est entièrement reçue
    /// </summary>
    public bool EstEntierementRecue => QteRecue >= QteLi;

    public int IdLigne { get; set; }

    public int RetourMarchandiseFournisseurId { get; set; }

    public string RefProduit { get; set; } = null!;

    public string DesignationLi { get; set; } = null!;

    public int QteLi { get; set; }

    public decimal PrixHt { get; set; }

    public double Remise { get; set; }

    public decimal TotHt { get; set; }

    public double Tva { get; set; }

    public decimal TotTtc { get; set; }

    /// <summary>
    /// Quantité reçue après réparation
    /// </summary>
    public int QteRecue { get; set; }

    /// <summary>
    /// Date de réception après réparation
    /// </summary>
    public DateTime? DateReception { get; set; }

    /// <summary>
    /// Utilisateur ayant validé la réception
    /// </summary>
    public string? UtilisateurReception { get; set; }

    public virtual RetourMarchandiseFournisseur RetourMarchandiseFournisseurNavigation { get; set; } = null!;

    public virtual Produit RefProduitNavigation { get; set; } = null!;
}
