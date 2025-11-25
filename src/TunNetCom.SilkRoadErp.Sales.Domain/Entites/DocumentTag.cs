#nullable enable

namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites;

public class DocumentTag
{
    public int Id { get; set; }

    public int TagId { get; set; }

    public string DocumentType { get; set; } = string.Empty;

    public int DocumentId { get; set; }

    public virtual Tag Tag { get; set; } = null!;
}

