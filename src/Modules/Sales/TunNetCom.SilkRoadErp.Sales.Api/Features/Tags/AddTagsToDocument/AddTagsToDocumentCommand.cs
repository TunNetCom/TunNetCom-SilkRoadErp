namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Tags.AddTagsToDocument;

public record AddTagsToDocumentCommand(
    string DocumentType,
    int DocumentId,
    List<int> TagIds
) : IRequest<Result>;

