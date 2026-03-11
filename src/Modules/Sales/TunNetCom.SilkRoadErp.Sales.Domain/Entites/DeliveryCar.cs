#nullable enable
using System.Collections.Generic;
using TunNetCom.SilkRoadErp.SharedKernel.Tenancy;

namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites;

public class DeliveryCar : ITenantEntity
{
    public int Id { get; set; }

    public string TenantId { get; set; } = TenantConstants.DefaultTenantId;

    public string Matricule { get; set; } = string.Empty;

    public string Owner { get; set; } = string.Empty;

    public virtual ICollection<BonDeLivraison> BonDeLivraisons { get; set; } = new List<BonDeLivraison>();
}






