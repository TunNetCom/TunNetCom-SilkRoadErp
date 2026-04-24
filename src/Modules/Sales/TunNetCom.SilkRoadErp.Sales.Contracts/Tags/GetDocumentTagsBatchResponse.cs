namespace TunNetCom.SilkRoadErp.Sales.Contracts.Tags;

public class GetDocumentTagsBatchResponse
{
    public string DocumentType { get; set; } = string.Empty;

    public List<DocumentTagResponse> Documents { get; set; } = new();
}

