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
        double tax)
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
            RetourMarchandiseFournisseurId = retourMarchandiseFournisseurId
        };
    }

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

    public virtual RetourMarchandiseFournisseur RetourMarchandiseFournisseurNavigation { get; set; } = null!;

    public virtual Produit RefProduitNavigation { get; set; } = null!;
}

