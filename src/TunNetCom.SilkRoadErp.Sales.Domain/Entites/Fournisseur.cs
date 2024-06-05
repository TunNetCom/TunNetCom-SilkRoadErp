﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable enable
using System;
using System.Collections.Generic;

namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites;

public partial class Fournisseur
{
    public int Id { get; set; }

    public string Nom { get; set; } = null!;

    public string Tel { get; set; } = null!;

    public string? Fax { get; set; }

    public string? Matricule { get; set; }

    public string? Code { get; set; }

    public string? CodeCat { get; set; }

    public string? EtbSec { get; set; }

    public string? Mail { get; set; }

    public string? MailDeux { get; set; }

    public bool Constructeur { get; set; }

    public string? Adresse { get; set; }

    public virtual ICollection<AvoirFournisseur> AvoirFournisseur { get; set; } = new List<AvoirFournisseur>();

    public virtual ICollection<BonDeReception> BonDeReception { get; set; } = new List<BonDeReception>();

    public virtual ICollection<Commandes> Commandes { get; set; } = new List<Commandes>();

    public virtual ICollection<EcheanceDesFournisseurs> EcheanceDesFournisseurs { get; set; } = new List<EcheanceDesFournisseurs>();

    public virtual ICollection<FactureAvoirFournisseur> FactureAvoirFournisseur { get; set; } = new List<FactureAvoirFournisseur>();

    public virtual ICollection<FactureFournisseur> FactureFournisseur { get; set; } = new List<FactureFournisseur>();
}