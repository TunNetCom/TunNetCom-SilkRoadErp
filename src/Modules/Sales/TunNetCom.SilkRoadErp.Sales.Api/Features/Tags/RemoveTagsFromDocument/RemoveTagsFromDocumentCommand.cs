namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Tags.RemoveTagsFromDocument;

public record RemoveTagsFromDocumentCommand(
    string DocumentType,
    int DocumentId,
    List<int> TagIds
) : IRequest<Result>;

