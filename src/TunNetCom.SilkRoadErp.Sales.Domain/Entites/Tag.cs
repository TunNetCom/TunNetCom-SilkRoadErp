#nullable enable
using System.Collections.Generic;

namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites;

public class Tag
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Color { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<DocumentTag> DocumentTags { get; set; } = new List<DocumentTag>();
}

