﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable enable
using System;
using System.Collections.Generic;

namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites;

public partial class BonDeLivraison
{
    public int Num { get; set; }

    public DateTime Date { get; set; }

    public decimal TotHTva { get; set; }

    public decimal TotTva { get; set; }

    public decimal NetPayer { get; set; }

    public TimeOnly TempBl { get; set; }

    public int? NumFacture { get; set; }

    public int? ClientId { get; set; }

    public virtual Client? Client { get; set; }

    public virtual ICollection<LigneBl> LigneBl { get; set; } = new List<LigneBl>();

    public virtual Facture? NumFactureNavigation { get; set; }

    public virtual Transaction? Transaction { get; set; }
}