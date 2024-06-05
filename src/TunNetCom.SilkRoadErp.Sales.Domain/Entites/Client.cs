﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable enable
using System;
using System.Collections.Generic;

namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites;

public partial class Client
{
    public int Id { get; set; }

    public string Nom { get; set; } = null!;

    public string? Tel { get; set; }

    public string? Adresse { get; set; }

    public string? Matricule { get; set; }

    public string? Code { get; set; }

    public string? CodeCat { get; set; }

    public string? EtbSec { get; set; }

    public string? Mail { get; set; }

    public virtual ICollection<Avoirs> Avoirs { get; set; } = new List<Avoirs>();

    public virtual ICollection<BonDeLivraison> BonDeLivraison { get; set; } = new List<BonDeLivraison>();

    public virtual ICollection<Devis> Devis { get; set; } = new List<Devis>();

    public virtual ICollection<Facture> Facture { get; set; } = new List<Facture>();
}