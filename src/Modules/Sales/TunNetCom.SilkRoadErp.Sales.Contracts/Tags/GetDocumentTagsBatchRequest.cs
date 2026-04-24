namespace TunNetCom.SilkRoadErp.Sales.Contracts.Tags;

public class GetDocumentTagsBatchRequest
{
    public string DocumentType { get; set; } = string.Empty;

    public List<int> DocumentIds { get; set; } = new();
}

