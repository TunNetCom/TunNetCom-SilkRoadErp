#nullable enable
using System.Collections.Generic;
using TunNetCom.SilkRoadErp.SharedKernel.Tenancy;

namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites;

public class Tag : ITenantEntity
{
    public int Id { get; set; }

    public string TenantId { get; set; } = TenantConstants.DefaultTenantId;

    public string Name { get; set; } = string.Empty;

    public string? Color { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<DocumentTag> DocumentTags { get; set; } = new List<DocumentTag>();
}

