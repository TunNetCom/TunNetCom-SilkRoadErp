﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable enable
using System;
using System.Collections.Generic;

namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites;

public partial class Avoirs
{
    public int Num { get; set; }

    public DateTime Date { get; set; }

    public int? ClientId { get; set; }

    public virtual Client? Client { get; set; }

    public virtual ICollection<LigneAvoirs> LigneAvoirs { get; set; } = new List<LigneAvoirs>();
}