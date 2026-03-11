#nullable enable
using TunNetCom.SilkRoadErp.SharedKernel.Tenancy;

namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites;

public class DocumentTag : ITenantEntity
{
    public int Id { get; set; }

    public string TenantId { get; set; } = TenantConstants.DefaultTenantId;

    public int TagId { get; set; }

    public string DocumentType { get; set; } = string.Empty;

    public int DocumentId { get; set; }

    public virtual Tag Tag { get; set; } = null!;
}

