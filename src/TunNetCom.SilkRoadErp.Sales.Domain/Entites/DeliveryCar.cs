#nullable enable
using System.Collections.Generic;

namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites;

public class DeliveryCar
{
    public int Id { get; set; }

    public string Matricule { get; set; } = string.Empty;

    public string Owner { get; set; } = string.Empty;

    public virtual ICollection<BonDeLivraison> BonDeLivraisons { get; set; } = new List<BonDeLivraison>();
}






