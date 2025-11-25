using TunNetCom.SilkRoadErp.Sales.Contracts.Tags;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Tags.GetDocumentTags;

public record GetDocumentTagsQuery(
    string DocumentType,
    int DocumentId
) : IRequest<DocumentTagResponse>;

