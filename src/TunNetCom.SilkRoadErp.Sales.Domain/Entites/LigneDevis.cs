﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable enable
using System;
using System.Collections.Generic;

namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites;

public partial class LigneDevis
{
    public int IdLi { get; set; }

    public int NumDevis { get; set; }

    public string RefProduit { get; set; } = null!;

    public string DesignationLi { get; set; } = null!;

    public int QteLi { get; set; }

    public decimal PrixHt { get; set; }

    public double Remise { get; set; }

    public decimal TotHt { get; set; }

    public double Tva { get; set; }

    public decimal TotTtc { get; set; }

    public virtual Devis NumDevisNavigation { get; set; } = null!;

    public virtual Produit RefProduitNavigation { get; set; } = null!;
}