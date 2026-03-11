namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Tags.AddTagsToDocumentByName;

public record AddTagsToDocumentByNameCommand(
    string DocumentType,
    int DocumentId,
    List<string> TagNames
) : IRequest<Result>;

